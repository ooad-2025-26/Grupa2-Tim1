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
using System.Linq;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Authorize] 
    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RezervacijaController(ApplicationDbContext context)
        {
            _db = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> KreirajRezervaciju([FromBody] NovaRezervacijaDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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
                return BadRequest($"Odabrani PaketId ({model.PaketId}) ne postoji u bazi podataka.");
            }

            decimal baznaCijena = paket.CijenaOd > 0 ? paket.CijenaOd : 500;

            if (model.Putnici == null || model.Putnici.Count == 0)
                return BadRequest(new { poruka = "Morate unijeti najmanje jednog putnika." });

            int brojPutnika = model.Putnici.Count;
            decimal konacnaCijena = baznaCijena * brojPutnika;
            int? primijenjenKuponId = null;

            if (model.DatumPovratka < model.DatumPolaska)
                return BadRequest(new { poruka = "Datum povratka ne može biti prije datuma polaska." });

            if (!string.IsNullOrEmpty(model.PromoKod))
            {
                var kupon = await _db.Kupon.FirstOrDefaultAsync(k => k.Kod.ToUpper() == model.PromoKod.ToUpper());
                if (kupon != null)
                {
                    if (kupon.Iskoristen) return BadRequest("Ovaj kupon je već iskorišten.");
                    if (kupon.VaziDo < DateTime.UtcNow) return BadRequest("Rok trajanja ovog kupona je istekao."); // Konzistentno UTC

                    decimal popust = konacnaCijena * (kupon.PopustProcenat / 100m);
                    konacnaCijena -= popust;

                    kupon.Iskoristen = true;
                    _db.Entry(kupon).State = EntityState.Modified;
                    primijenjenKuponId = kupon.Id;
                }
                else
                {
                    return BadRequest("Uneseni promo kod nije važeći.");
                }
            }

            if (paket.Kapacitet < brojPutnika)
            {
                return BadRequest(new { poruka = "Nema dovoljno slobodnih mjesta na ovom turističkom paketu." });
            }

            if (paket.HotelId.HasValue)
            {
                var hotel = await _db.Hoteli.FindAsync(paket.HotelId.Value);
                if (hotel == null || hotel.DostupnoSoba < brojPutnika)
                {
                    return BadRequest(new { poruka = "Nema dovoljno slobodnog kapaciteta u odabranom hotelu." });
                }
            }

            if (paket.DostupniPrevoz == VrstaPrevoza.SamoAvion || paket.DostupniPrevoz == VrstaPrevoza.Oboje)
            {
                if (paket.LetId.HasValue)
                {
                    var let = await _db.Letovi.FindAsync(paket.LetId.Value);
                    if (let == null || let.SlobodnaSjedista < brojPutnika)
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

                        Putnici = model.Putnici.Select(p => new Putnik
                        {
                            Ime = p.Ime,
                            Prezime = p.Prezime,
                            BrojPasosa = p.BrojPasosa,
                            Drzavljanstvo = p.Drzavljanstvo,
                            DatumRodjenja = p.DatumRodjenja,
                            PosebniZahtjevi = p.PosebniZahtjevi,
                            Telefon = p.Telefon
                        }).ToList()
                    };

                    _db.Rezervacije.Add(novaRezervacija);
                    await _db.SaveChangesAsync();

                    MetodaPlacanja enumMetoda = MetodaPlacanja.Kartica;
                    if (model.NacinPlacanja == "rates")
                        enumMetoda = MetodaPlacanja.Rate;

                    var novoPlacanje = new Placanje
                    {
                        RezervacijaId = novaRezervacija.Id,
                        Iznos = model.UkupanIznos > 0 ? model.UkupanIznos : konacnaCijena,
                        Metoda = enumMetoda,
                        KuponId = primijenjenKuponId,
                        VrijemePlacanja = DateTime.UtcNow
                    };

                    _db.Placanja.Add(novoPlacanje);
                    await _db.SaveChangesAsync();

                    if (model.NacinPlacanja == "rates" && model.BrojMjeseciRata.HasValue && model.BrojMjeseciRata.Value > 0)
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

                    var trackedPaket = await _db.Paketi.FindAsync(paket.Id);
                    if (trackedPaket != null)
                    {
                        trackedPaket.Kapacitet -= brojPutnika;
                        _db.Entry(trackedPaket).State = EntityState.Modified;

                        if (trackedPaket.HotelId.HasValue)
                        {
                            var hotel = await _db.Hoteli.FindAsync(trackedPaket.HotelId.Value);
                            if (hotel != null)
                            {
                                hotel.DostupnoSoba -= brojPutnika;
                                _db.Entry(hotel).State = EntityState.Modified;
                            }
                        }

                        if (trackedPaket.DostupniPrevoz == VrstaPrevoza.SamoAvion || trackedPaket.DostupniPrevoz == VrstaPrevoza.Oboje)
                        {
                            if (trackedPaket.LetId.HasValue)
                            {
                                var let = await _db.Letovi.FindAsync(trackedPaket.LetId.Value);
                                if (let != null)
                                {
                                    let.SlobodnaSjedista -= brojPutnika;
                                    _db.Entry(let).State = EntityState.Modified;
                                }
                            }
                        }
                    }

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    novoPlacanje.TransakcijskiKod = "TRX-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                    _db.Entry(novoPlacanje).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    PozadinskiProcesiService.DodajRezervacijuURed(novaRezervacija.Id);

                    return Ok(new
                    {
                        id = novaRezervacija.Id,
                        cijenaZaPlacanje = novoPlacanje.Iznos,
                        message = "Rezervacija uspješno kreirana, kapaciteti ažurirani!"
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { poruka = "Greška prilikom čuvanja podataka: " + ex.Message });
                }
            });
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
                    .Include(r => r.Putnici)
                    .Where(r => r.KorisnikId == korisnik.Id)
                    .ToListAsync();

                var rezultat = siroveRezervacije.Select(r => new
                {
                    id = r.Id,
                    status = r.Status.ToString(),
                    destinacija = r.Paket?.Destinacija?.Naziv ?? "—",
                    drzava = r.Paket?.Destinacija?.Drzava ?? "—",
                    paketNaziv = r.Paket?.Naziv ?? "—",
                    paketId = r.PaketId,
                    datumPolaska = r.DatumPolaska.ToString("dd.MM.yyyy"),
                    datumPovratka = r.DatumPovratka.ToString("dd.MM.yyyy"),
                    brojPutnika = r.Putnici != null ? r.Putnici.Count : 1,

                    putnici = (r.Putnici ?? new List<Putnik>()).Select(p => new
                    {
                        ime = p.Ime,
                        prezime = p.Prezime,
                        brojPasosa = p.BrojPasosa ?? "—",
                        drzavljanstvo = p.Drzavljanstvo ?? "—",
                        telefon = p.Telefon ?? "—",
                        datumRodjenja = p.DatumRodjenja.HasValue ? p.DatumRodjenja.Value.ToString("dd.MM.yyyy") : "—",
                        posebniZahtjevi = p.PosebniZahtjevi ?? "Nema"
                    }).ToList()
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

            return await strategy.ExecuteAsync<IActionResult>(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var emailKorisnika = User.Identity?.Name;
                    var korisnik = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
                    if (korisnik == null) return Unauthorized("Korisnik nije pronađen u sistemu.");

                    var rezervacija = await _db.Rezervacije
                        .Include(r => r.Putnici)
                        .Include(r => r.Paket)
                        .FirstOrDefaultAsync(r => r.Id == id && r.KorisnikId == korisnik.Id);

                    if (rezervacija == null) return NotFound("Rezervacija nije pronađena.");
                    if (rezervacija.Status == StatusRezervacije.Otkazana) return BadRequest("Rezervacija je već otkazana.");

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

                    return Ok(new { message = "Rezervacija je uspješno otkazana, a kapaciteti i kupon su oslobođeni." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { greska = "Greška prilikom otkazivanja: " + ex.Message });
                }
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProvjeriKupon(string kod, int paketId)
        {
            if (string.IsNullOrEmpty(kod)) return BadRequest("Kod nije unesen.");
            kod = kod.Trim();

            var paket = await _db.Paketi.FindAsync(paketId);
            if (paket == null) return BadRequest("Odabrani paket ne postoji.");

            decimal staraCijena = paket.CijenaOd > 0 ? paket.CijenaOd : 500;

            var kupon = await _db.Kupon.FirstOrDefaultAsync(k => k.Kod.Trim().ToUpper() == kod.ToUpper());

            if (kupon != null)
            {
                if (kupon.Iskoristen)
                    return Ok(new { validan = false, poruka = "Ovaj kupon je već iskorišten." });

                if (kupon.VaziDo < DateTime.Now)
                    return Ok(new { validan = false, poruka = "Rok trajanja kupona je istekao." });

                decimal popustIznos = staraCijena * (kupon.PopustProcenat / 100m);
                decimal novaCijena = staraCijena - popustIznos;

                return Ok(new
                {
                    validan = true,
                    poruka = $"Kupon uspješno primijenjen! Popust: {kupon.PopustProcenat}%",
                    novaCijena = novaCijena,
                    usteda = popustIznos,
                    popustProcenat = kupon.PopustProcenat
                });
            }

            return NotFound(new { validan = false, poruka = "Uneseni kupon ne postoji u bazi podataka." });
        }
        [HttpGet]
        public async Task<IActionResult> PreuzmiItinerer(int id)
        {
            var emailKorisnika = User.Identity?.Name;
            if (string.IsNullOrEmpty(emailKorisnika)) return Unauthorized();

            var korisnik = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);
            if (korisnik == null) return Unauthorized();

            var rezervacija = await _db.Rezervacije
                .Include(r => r.Paket).ThenInclude(p => p!.Hotel)
                .Include(r => r.Paket).ThenInclude(p => p!.Let)
                .Include(r => r.Putnici)
                .FirstOrDefaultAsync(r => r.Id == id && r.KorisnikId == korisnik.Id);

            if (rezervacija == null)
            {
                return NotFound("Rezervacija nije pronađena.");
            }

            var placanje = await _db.Placanja.FirstOrDefaultAsync(p => p.RezervacijaId == rezervacija.Id);
            var rate = placanje != null ? await _db.RatePlacanja.Where(rp => rp.PlacanjeId == placanje.Id).ToListAsync() : new List<RataPlacanja>();

            byte[] pdfBytes;
            try
            {
                var emailService = HttpContext.RequestServices.GetRequiredService<EmailAndDocumentService>();

                string ispravanDatum = DateTime.Now.ToString("dd.MM.yyyy.");
                string resCode = $"IT-{rezervacija.Id:D4}-{rezervacija.DatumPolaska.Year}";

                string prevozIspis = "Autobuski prevoz uključen u cijenu";
                if (rezervacija.Paket != null && (rezervacija.Paket.DostupniPrevoz == VrstaPrevoza.SamoAvion || rezervacija.Paket.DostupniPrevoz == VrstaPrevoza.Oboje))
                {
                    if (rezervacija.Paket.Let != null)
                    {
                        string relacija = (!string.IsNullOrEmpty(rezervacija.Paket.Let.Polazak) && !string.IsNullOrEmpty(rezervacija.Paket.Let.Odrediste))
                            ? $"({rezervacija.Paket.Let.Polazak} - {rezervacija.Paket.Let.Odrediste})"
                            : "";
                        prevozIspis = $"Avion — Let: {rezervacija.Paket.Let.BrojLeta} {relacija}".Trim();
                    }
                    else
                    {
                        prevozIspis = "Avionski prevoz uključen u cijenu";
                    }
                }

                string nacinPlacanjaLabel = "Kartica / Jednokratno";
                bool isRates = false;
                if (placanje != null && placanje.Metoda == MetodaPlacanja.Rate)
                {
                    nacinPlacanjaLabel = "Plaćanje na rate";
                    isRates = true;
                }

                decimal ukupanIznos = placanje?.Iznos ?? 0;
                int brojMjeseci = rate.Count;
                decimal iznosRate = brojMjeseci > 0 ? (rate.FirstOrDefault()?.IznosRate ?? 0) : 0;

                string kontaktTelefon = rezervacija.Putnici?.FirstOrDefault()?.Telefon ?? "—";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append($@"
            <div class='doc-title'>POTVRDA REZERVACIJE</div>
            <div class='doc-meta'>
                Broj dokumenta: {resCode}<br />
                Datum izdavanja: {ispravanDatum}
            </div>
            <div class='divider'></div>

            <table class='grid-container'>
                <tr>
                    <td class='grid-col'>
                        <div class='section-title'>DETALJI PUTOVANJA:</div>
                        <div class='info-line'><b>Destinacija:</b> {rezervacija.Paket?.Naziv ?? "—"}</div>
                        <div class='info-line'><b>Period:</b> {rezervacija.DatumPolaska.ToString("dd.MM.yyyy")} - {rezervacija.DatumPovratka.ToString("dd.MM.yyyy")}</div>
                        <div class='info-line'><b>Smještaj:</b> {rezervacija.Paket?.Hotel?.Naziv ?? "Standardni smještaj"}</div>
                        <div class='info-line'><b>Broj putnika:</b> {rezervacija.Putnici?.Count ?? 1}</div>
                        <div class='info-line'><b>Prevoz:</b> {prevozIspis}</div>
                        <div class='info-line'><b>Kontakt telefon:</b> {kontaktTelefon}</div>
                    </td>
                    <td class='grid-col' style='padding-left: 20px;'>
                        <div class='section-title'>FINANSIJSKI PODACI:</div>
                        <div class='info-line'><b>Način plaćanja:</b> {nacinPlacanjaLabel}</div>
                        <div class='total-amount'>UKUPNO: {ukupanIznos:F2} KM</div>");

                if (isRates)
                {
                    sb.Append($@"
                        <div class='info-line' style='margin-top: 10px;'><b>Mjeseci:</b> {brojMjeseci}</div>
                        <div class='info-line'><b>Mjesečna rata:</b> {iznosRate:F2} KM</div>");
                }

                sb.Append($@"
                    </td>
                </tr>
            </table>

            <div class='divider' style='margin-top: 10px;'></div>
            <div class='table-title'>PODACI O PUTNICIMA:</div>
            <table class='passengers-table'>
                <thead>
                    <tr>
                        <th style='width: 5%;'>#</th>
                        <th style='width: 30%;'>Ime i Prezime</th>
                        <th style='width: 12%;'>Tip</th>
                        <th style='width: 15%;'>Datum rođ.</th>
                        <th style='width: 15%;'>Pasoš</th>
                        <th style='width: 10%;'>Drž.</th>
                        <th style='width: 13%;'>Telefon</th>
                    </tr>
                </thead>
                <tbody>");

                if (rezervacija.Putnici == null || rezervacija.Putnici.Count == 0)
                {
                    sb.Append("<tr><td colspan='7' style='text-align:center;'>Nema sačuvanih podataka o putnicima.</td></tr>");
                }
                else
                {
                    int rb = 1;
                    foreach (var p in rezervacija.Putnici)
                    {
                        string tipPutnika = "Odrasli";
                        if (p.DatumRodjenja.HasValue && p.DatumRodjenja.Value > DateTime.Now.AddYears(-12))
                        {
                            tipPutnika = "Dijete";
                        }

                        string rodjendan = p.DatumRodjenja.HasValue ? p.DatumRodjenja.Value.ToString("dd.MM.yyyy.") : "—";

                        sb.Append($@"
                    <tr>
                        <td>{rb}.</td>
                        <td>
                            <b>{p.Ime} {p.Prezime}</b>");

                        if (!string.IsNullOrEmpty(p.PosebniZahtjevi))
                        {
                            sb.Append($"<div class='note-line'>Napomena: {p.PosebniZahtjevi}</div>");
                        }

                        sb.Append($@"
                        </td>
                        <td>{tipPutnika}</td>
                        <td>{rodjendan}</td>
                        <td>{p.BrojPasosa ?? "—"}</td>
                        <td>{p.Drzavljanstvo ?? "—"}</td>
                        <td>{p.Telefon ?? "—"}</td>
                    </tr>");
                        rb++;
                    }
                }

                sb.Append($@"
                </tbody>
            </table>

            <table class='footer-container'>
                <tr>
                    <td class='stamp-col'>
                        <div class='stamp-circle'>
                            <div class='stamp-text'>
                                INTERTRIPS d.o.o.<br />
                                SARAJEVO<br />
                                Faktura / Potvrda
                            </div>
                        </div>
                    </td>
                    <td class='signature-col'>
                        <div class='signature-line'></div>
                        <div style='font-size: 11px;'>Ovlašteno lice</div>
                    </td>
                </tr>
            </table>

            <div class='legal-footer'>
                Ovaj dokument je validan bez pečata i potpisa ukoliko je generisan elektronskim putem.
            </div>");

                pdfBytes = emailService.GenerisiPdfDokument($"POTVRDA_{resCode}", sb.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška pri generisanju PDF-a: {ex.Message}");
            }

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return BadRequest("PDF dokument je prazan.");
            }

            return File(pdfBytes, "application/pdf", $"Potvrda_InterTrips_IT-{rezervacija.Id}.pdf");
        }
    }
}