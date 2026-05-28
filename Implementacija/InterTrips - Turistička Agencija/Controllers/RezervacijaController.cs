using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Enums;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace InterTrips___Turistička_Agencija.Controllers
{
    [Authorize]

    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailAndDocumentService _emailService;
        public RezervacijaController(ApplicationDbContext db, EmailAndDocumentService emailService)
        {
            _db = db;
            _emailService = emailService; 
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("~/Views/Rezervacija/Index.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Rezervacija(int? paketId)
        {
            var paketi = await _db.Paketi
                .Include(p => p.Destinacija)
                .Include(p => p.DostupniTermini)
                .Include(p => p.Hotel)
                .Include(p => p.Let)
                .Where(p => p.Status != StatusPaketa.Rasprodan)
                .ToListAsync(); 

            ViewBag.SviPaketi = paketi ?? new List<Paket>();

            Paket? selektovaniPaket = null;
            if (paketId.HasValue && paketi != null)
            {
                selektovaniPaket = paketi.FirstOrDefault(p => p.Id == paketId.Value);
            }

            if (selektovaniPaket == null && paketi != null && paketi.Any())
            {
                selektovaniPaket = paketi.First();
            }

            ViewBag.SelektovaniPaketId = selektovaniPaket?.Id ?? 0;

            return View("~/Views/Rezervacija/Rezervacija.cshtml", paketi);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Putnici()
        {
            return View("~/Views/Rezervacija/Putnici.cshtml");
        }


        public class PutnikDto
        {
            [Required(ErrorMessage = "Ime je obavezno.")]
            public string Ime { get; set; } = string.Empty;

            [Required(ErrorMessage = "Prezime je obavezno.")]
            public string Prezime { get; set; } = string.Empty;

            [Required(ErrorMessage = "Pol je obavezan.")]
            public string Pol { get; set; } = string.Empty; 

            [Required(ErrorMessage = "Broj pasoša je obavezan.")]
            public string BrojPasosa { get; set; } = string.Empty;

            [Required(ErrorMessage = "Državljanstvo je obavezno.")]
            public string Drzavljanstvo { get; set; } = string.Empty;

            [Required(ErrorMessage = "Datum rođenja je obavezan.")]
            public DateTime? DatumRodjenja { get; set; }

            public string? Telefon { get; set; }
            public string? PosebniZahtjevi { get; set; }
        }
    


    [HttpPost]
    public async Task<IActionResult> KreirajRezervaciju([FromBody] NovaRezervacijaDto model)
    {
        if (!ModelState.IsValid)
        {
            var greske = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(new { poruka = "Validacija nije prošla: " + greske });
        }

        if (model.Putnici == null || model.Putnici.Count == 0)
            return BadRequest(new { poruka = "Morate unijeti najmanje jednog putnika." });

        int brojPutnika = model.Putnici.Count;

        string? emailKorisnika = User.Identity?.Name;
        if (string.IsNullOrEmpty(emailKorisnika)) return Unauthorized("Korisnik nije ulogovan.");

        var korisnik = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
        if (korisnik == null) return Unauthorized("Korisnik nije pronađen u bazi podataka.");

        var paket = await _db.Paketi
            .Include(p => p.Hotel)
            .Include(p => p.Let)
            .FirstOrDefaultAsync(p => p.Id == model.PaketId);

        if (paket == null)
        {
            return BadRequest(new { poruka = $"Odabrani PaketId ({model.PaketId}) ne postoji u bazi podataka." });
        }

        if (model.DatumPovratka < model.DatumPolaska)
            return BadRequest(new { poruka = "Datum povratka ne može biti prije datuma polaska." });

        decimal baznaCijena = paket.CijenaOd > 0 ? paket.CijenaOd : 500;
        decimal konacnaCijena = baznaCijena * brojPutnika;
        int? primijenjenKuponId = null;

        Kupon? kupon = null;

        if (!string.IsNullOrWhiteSpace(model.PromoKod) && model.PromoKod.Trim().ToLower() != "null")
        {
            kupon = await _db.Kupon.FirstOrDefaultAsync(k => k.Kod.Trim().ToUpper() == model.PromoKod.Trim().ToUpper());

            if (kupon == null)
                return BadRequest(new { poruka = "Uneseni promo kod nije važeći." });

            if (kupon.Iskoristen)
                return BadRequest(new { poruka = "Ovaj kupon je već iskorišten." });

            if (kupon.VaziDo < DateTime.UtcNow)
                return BadRequest(new { poruka = "Rok trajanja ovog kupona je istekao." });

            decimal popust = konacnaCijena * (kupon.PopustProcenat / 100m);
            konacnaCijena -= popust;
            primijenjenKuponId = kupon.Id;
        }

        decimal iznosZaPlacanje = kupon != null ? konacnaCijena : (model.UkupanIznos > 0 ? model.UkupanIznos : konacnaCijena);

        if (paket.Kapacitet < brojPutnika)
        {
            return BadRequest(new { poruka = "Nema dovoljno slobodnih mjesta na ovom turističkom paketu." });
        }

        if (paket.HotelId.HasValue)
        {
            if (paket.Hotel == null || paket.Hotel.DostupnoSoba < brojPutnika)
            {
                return BadRequest(new { poruka = "Nema dovoljno slobodnog kapaciteta u odabranom hotelu." });
            }
        }

        if (paket.DostupniPrevoz == VrstaPrevoza.SamoAvion || paket.DostupniPrevoz == VrstaPrevoza.Oboje)
        {
            if (paket.LetId.HasValue)
            {
                if (paket.Let == null || paket.Let.SlobodnaSjedista < brojPutnika)
                {
                    return BadRequest(new { poruka = "Nema dovoljno slobodnih sjedišta na odabranom letu." });
                }
            }
        }

        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var novaRezervacija = new Rezervacija
                {
                    PaketId = model.PaketId,
                    DatumPolaska = model.DatumPolaska,
                    DatumPovratka = model.DatumPovratka,
                    Status = StatusRezervacije.Kreirana,
                    KorisnikId = korisnik.Id,
                    TipSobe = model.TipSobe ?? "Standardna soba",
                    TipPrevoza = model.TipPrevoza == "Avion" ? VrstaPrevoza.SamoAvion : VrstaPrevoza.SamoAutobus,
                    Putnici = model.Putnici.Select(p => new Putnik
                    {
                        Ime = p.Ime,
                        Prezime = p.Prezime,
                        BrojPasosa = p.BrojPasosa,
                        Drzavljanstvo = p.Drzavljanstvo,
                        DatumRodjenja = p.DatumRodjenja.GetValueOrDefault(DateTime.Now), 
                        PosebniZahtjevi = p.PosebniZahtjevi,
                        Telefon = p.Telefon,
                        Pol = p.Pol
                    }).ToList()
                };

                _db.Rezervacije.Add(novaRezervacija);
                await _db.SaveChangesAsync();

                MetodaPlacanja enumMetoda = model.NacinPlacanja == "rates" ? MetodaPlacanja.Rate : MetodaPlacanja.Kartica;

                var novoPlacanje = new Placanje
                {
                    RezervacijaId = novaRezervacija.Id,
                    Iznos = iznosZaPlacanje,
                    Metoda = enumMetoda,
                    KuponId = primijenjenKuponId,
                    VrijemePlacanja = DateTime.UtcNow,
                    TransakcijskiKod = "TRX-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };

                _db.Placanja.Add(novoPlacanje);
                await _db.SaveChangesAsync();

                if (enumMetoda == MetodaPlacanja.Rate && model.BrojMjeseciRata.HasValue && model.BrojMjeseciRata.Value > 0)
                {
                    decimal iznosRate = novoPlacanje.Iznos / model.BrojMjeseciRata.Value;

                    for (int i = 1; i <= model.BrojMjeseciRata.Value; i++)
                    {
                        _db.RatePlacanja.Add(new RataPlacanja
                        {
                            PlacanjeId = novoPlacanje.Id,
                            IznosRate = iznosRate,
                            RokZaUplatu = DateTime.UtcNow.AddMonths(i),
                            IsUplaceno = false
                        });
                    }
                    await _db.SaveChangesAsync();
                }

                paket.Kapacitet -= brojPutnika;
                _db.Entry(paket).State = EntityState.Modified;

                if (paket.Hotel != null)
                {
                    paket.Hotel.DostupnoSoba -= brojPutnika;
                    _db.Entry(paket.Hotel).State = EntityState.Modified;
                }

                if ((paket.DostupniPrevoz == VrstaPrevoza.SamoAvion || paket.DostupniPrevoz == VrstaPrevoza.Oboje) && paket.Let != null)
                {
                    paket.Let.SlobodnaSjedista -= brojPutnika;
                    _db.Entry(paket.Let).State = EntityState.Modified;
                }

                if (kupon != null)
                {
                    kupon.Iskoristen = true;
                    _db.Entry(kupon).State = EntityState.Modified;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                PozadinskiProcesiService.DodajRezervacijuURed(novaRezervacija.Id);

                return Ok(new
                {
                    id = novaRezervacija.Id,
                    cijenaZaPlacanje = novoPlacanje.Iznos,
                    transakcijskiKod = novoPlacanje.TransakcijskiKod,
                    message = "Rezervacija uspješno kreirana, kapaciteti ažurirani!"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { poruka = "Greška prilikom čuvanja podataka i ažuriranja kapaciteta: " + ex.Message });
            }
        });
    }
    [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProvjeriKupon(string kod, int paketId, int brojPutnika = 1)
        {
            if (string.IsNullOrWhiteSpace(kod))
                return BadRequest(new { poruka = "Kod nije unesen." });

            kod = kod.Trim();

            var paket = await _db.Paketi.FindAsync(paketId);
            if (paket == null)
                return BadRequest(new { poruka = "Odabrani paket ne postoji." });

            if (brojPutnika < 1)
                brojPutnika = 1;

            decimal cijenaPoOsobi = paket.CijenaOd > 0 ? paket.CijenaOd : 500;
            decimal osnovnaUkupnaCijena = cijenaPoOsobi * brojPutnika;

            var kupon = await _db.Kupon.FirstOrDefaultAsync(k => k.Kod.Trim().ToUpper() == kod.ToUpper());

            if (kupon != null)
            {
                if (kupon.Iskoristen)
                    return BadRequest(new { poruka = "Ovaj kupon je već iskorišten." });

                if (kupon.VaziDo < DateTime.UtcNow)
                    return BadRequest(new { poruka = "Rok trajanja ovog kupona je istekao." });

                decimal popustIznos = osnovnaUkupnaCijena * (kupon.PopustProcenat / 100m);
                decimal novaUkupnaCijena = osnovnaUkupnaCijena - popustIznos;

                return Ok(new
                {
                    validan = true,
                    poruka = $"Kupon uspješno primijenjen! Popust: {kupon.PopustProcenat}%",
                    cijenaPoOsobi = cijenaPoOsobi,
                    osnovnaUkupnaCijena = osnovnaUkupnaCijena,
                    novaUkupnaCijena = novaUkupnaCijena,
                    usteda = popustIznos,
                    popustProcenat = kupon.PopustProcenat
                });
            }

            return NotFound(new { poruka = "Uneseni kupon ne postoji u bazi podataka." });
        }

        [HttpGet]
        public async Task<IActionResult> GetMojeRezervacije()
        {
            try
            {
                var emailKorisnika = User.Identity?.Name;
                if (string.IsNullOrEmpty(emailKorisnika))
                {
                    return Unauthorized(new { greska = "Korisnik nije autorizovan. Molimo prijavite se ponovo." });
                }

                var korisnik = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
                if (korisnik == null)
                {
                    return NotFound(new { greska = "Korisnik ne postoji u Identity bazi podataka." });
                }

                var siroveRezervacije = await _db.Rezervacije
                    .Include(r => r.Paket)
                        .ThenInclude(p => p!.Destinacija)
                    .Include(r => r.Paket)
                        .ThenInclude(p => p!.Hotel)
                    .Include(r => r.Paket)
                        .ThenInclude(p => p!.Let)
                    .Include(r => r.Putnici)
                    .Where(r => r.KorisnikId == korisnik.Id)
                    .ToListAsync();

                var rezultat = siroveRezervacije.Select(r => {
                    string resCode = $"IT-{r.Id:D4}-{r.DatumPolaska.Year}";

                    string stvarniPrevoz = "Uključen prevoz";
                    if (r.TipPrevoza == VrstaPrevoza.SamoAvion || r.TipPrevoza.ToString() == "SamoAvion")
                    {
                        stvarniPrevoz = "Avion";
                    }
                    else if (r.TipPrevoza == VrstaPrevoza.SamoAutobus || r.TipPrevoza.ToString() == "SamoAutobus")
                    {
                        stvarniPrevoz = "Autobus";
                    }
                    return new
                    {
                        id = r.Id,
                        resCode = resCode,
                        status = r.Status.ToString(),
                        destinacija = r.Paket?.Destinacija?.Naziv ?? "—",
                        drzava = r.Paket?.Destinacija?.Drzava ?? "—",
                        paketNaziv = r.Paket?.Naziv ?? "—",
                        paketId = r.PaketId,
                        datumPolaska = r.DatumPolaska.ToString("dd.MM.yyyy"),
                        datumPovratka = r.DatumPovratka.ToString("dd.MM.yyyy"),
                        period = $"{r.DatumPolaska:dd.MM.yyyy} - {r.DatumPovratka:dd.MM.yyyy}",
                        brojPutnika = r.Putnici != null ? r.Putnici.Count : 1,

                        hotelNaziv = r.Paket?.Hotel?.Naziv ?? "Smještaj po programu",
                        odabranaSoba = r.TipSobe.ToString(), 

                        odabraniPrevoz = stvarniPrevoz,

                        putnici = (r.Putnici ?? new List<Putnik>()).Select(p => new
                        {
                            ime = p.Ime,
                            prezime = p.Prezime,
                            brojPasosa = p.BrojPasosa ?? "—",
                            drzavljanstvo = p.Drzavljanstvo ?? "—",
                            telefon = p.Telefon ?? "—",
                            datumRodjenja = p.DatumRodjenja.ToString("dd.MM.yyyy"),
                            posebniZahtjevi = p.PosebniZahtjevi ?? "Nema",

                            pol = p.Pol ?? "—",
                            tipPutnika = p.TipPutnika ?? "Odrasli"
                        }).ToList()
                    };
                }).ToList();

                return Ok(rezultat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { greska = ex.Message, unutrasnjaGreska = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtkaziRezervaciju(int id)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            string emailKorisnikaZaNotifikaciju = null;
            string nazivPaketaZaNotifikaciju = "Nije specificirano";

            var ishodTransakcije = await strategy.ExecuteAsync<bool>(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var emailKorisnika = User.Identity?.Name;
                    var korisnik = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
                    if (korisnik == null) return false;

                    var rezervacija = await _db.Rezervacije
                        .Include(r => r.Putnici)
                        .Include(r => r.Paket)
                        .FirstOrDefaultAsync(r => r.Id == id && r.KorisnikId == korisnik.Id);

                    if (rezervacija == null || rezervacija.Status == StatusRezervacije.Otkazana) return false;

                    emailKorisnikaZaNotifikaciju = korisnik.Email;
                    nazivPaketaZaNotifikaciju = rezervacija.Paket?.Naziv ?? "Nije specificirano";

                    var placanje = await _db.Placanja.FirstOrDefaultAsync(p => p.RezervacijaId == rezervacija.Id);
                    if (placanje != null && placanje.KuponId.HasValue)
                    {
                        var kuponZaVracanje = await _db.Kupon.FindAsync(placanje.KuponId.Value);
                        if (kuponZaVracanje != null)
                        {
                            kuponZaVracanje.Iskoristen = false;
                            _db.Entry(kuponZaVracanje).State = EntityState.Modified;
                        }
                    }

                    int brojPutnika = rezervacija.Putnici?.Count ?? 1;
                    var paket = rezervacija.Paket;

                    if (paket != null)
                    {
                        paket.Kapacitet += brojPutnika;
                        _db.Entry(paket).State = EntityState.Modified;

                        if (paket.HotelId.HasValue)
                        {
                            var hotel = await _db.Hoteli.FindAsync(paket.HotelId.Value);
                            if (hotel != null)
                            {
                                hotel.DostupnoSoba += brojPutnika;
                                _db.Entry(hotel).State = EntityState.Modified;
                            }
                        }

                        if (paket.DostupniPrevoz == VrstaPrevoza.SamoAvion || paket.DostupniPrevoz == VrstaPrevoza.Oboje)
                        {
                            if (paket.LetId.HasValue)
                            {
                                var let = await _db.Letovi.FindAsync(paket.LetId.Value);
                                if (let != null)
                                {
                                    let.SlobodnaSjedista += brojPutnika;
                                    _db.Entry(let).State = EntityState.Modified;
                                }
                            }
                        }
                    }

                    rezervacija.Status = StatusRezervacije.Otkazana;
                    _db.Entry(rezervacija).State = EntityState.Modified;

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            });

            if (!ishodTransakcije)
            {
                return BadRequest(new { greska = "Greška prilikom otkazivanja rezervacije ili rezervacija ne postoji." });
            }

            if (!string.IsNullOrEmpty(emailKorisnikaZaNotifikaciju))
            {
                string naslovEmaila = "Otkazivanje rezervacije - InterTrips";
                string sadrzajEmaila = $@"
            <h3>Poštovani,</h3>
            <p>Obavještavamo Vas da je Vaša rezervacija za aranžman <strong>{nazivPaketaZaNotifikaciju}</strong> uspješno otkazana.</p>
            <p>Sva rezervisana mjesta i iskorišteni kuponi (ukoliko ih je bilo) su vraćeni u sistem.</p>
            <br>
            <p>Srdačan pozdrav,<br>Vaš <strong>InterTrips</strong> tim</p>";

                _ = Task.Run(async () =>
                {
                    await _emailService.PosaljiEmailSaLogomAsync(
                        emailKorisnikaZaNotifikaciju,
                        naslovEmaila,
                        sadrzajEmaila,
                        id,
                        "OtkazivanjeRezervacije"
                    );
                });
            }

            return Ok(new { message = "Rezervacija je uspješno otkazana, kapaciteti su oslobođeni, a e-mail potvrda je poslana." });
        }


        [HttpGet]
        [Route("Rezervacija/PreuzmiItinerer/{id}")]
        public async Task<IActionResult> PreuzmiItinerer(int id)
        {
            string? emailKorisnika = User.Identity?.Name;
            if (string.IsNullOrEmpty(emailKorisnika)) return Unauthorized();

            var klijent = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
            if (klijent == null) return Unauthorized();

            var rezervacija = await _db.Rezervacije
                .Include(r => r.Paket).ThenInclude(p => p!.Hotel)
                .Include(r => r.Paket).ThenInclude(p => p!.Let)
                .Include(r => r.Putnici)
                .FirstOrDefaultAsync(r => r.Id == id && r.KorisnikId == klijent.Id);

            if (rezervacija == null) return NotFound("Rezervacija nije pronađena.");

            string resCode = $"IT-{rezervacija.Id:D4}-{rezervacija.DatumPolaska.Year}";
            string verifikacijaUrl = $"https://intertrips.ba/Verifikacija/Rezervacija/{rezervacija.Id}";

            string base64Qr = string.Empty;
            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(verifikacijaUrl, QRCoder.QRCodeGenerator.ECCLevel.H))
            using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
            {
                byte[] qrBytes = qrCode.GetGraphic(20);
                base64Qr = Convert.ToBase64String(qrBytes);
            }

            var emailService = HttpContext.RequestServices.GetRequiredService<EmailAndDocumentService>();
            byte[] pdfBytes = emailService.GenerisiPdfDokument($"POTVRDA_{resCode}", rezervacija, base64Qr);

            return File(pdfBytes, "application/pdf", $"Potvrda_InterTrips_{resCode}.pdf");
        }
    }
}