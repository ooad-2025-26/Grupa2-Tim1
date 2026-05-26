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
                    if (kupon.VaziDo < DateTime.UtcNow) return BadRequest("Rok trajanja ovog kupona je istekao.");

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

                    MetodaPlacanja enumMetoda = model.NacinPlacanja == "rates" ? MetodaPlacanja.Rate : MetodaPlacanja.Kartica;

                    var novoPlacanje = new Placanje
                    {
                        RezervacijaId = novaRezervacija.Id,
                        Iznos = model.UkupanIznos > 0 ? model.UkupanIznos : konacnaCijena,
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

            var placanje = await _db.Placanja.FirstOrDefaultAsync(p => p.RezervacijaId == rezervacija.Id);

            string ispravanDatum = DateTime.Now.ToString("dd.MM.yyyy.");
            string resCode = $"IT-{rezervacija.Id:D4}-{rezervacija.DatumPolaska.Year}";
            string nazivPaketa = rezervacija.Paket?.Naziv ?? "Nije specificirano";
            string nazivHotela = rezervacija.Paket?.Hotel?.Naziv ?? "Uključen smještaj po programu";
            string periodPutovanja = $"{rezervacija.DatumPolaska:dd.MM.yyyy} - {rezervacija.DatumPovratka:dd.MM.yyyy}";
            int brojPutnika = rezervacija.Putnici?.Count ?? 0;
            string ukupnaCijena = placanje != null ? $"{placanje.Iznos:F2} BAM" : "Na upit";
            string statusPlacanja = placanje != null ? "PLAĆENO (Potvrđeno)" : "ČEKA SE UPLATA";
            string qrCodeUrl = $"https://chart.googleapis.com/chart?chs=100x100&cht=qr&chl=https://intertrips.ba/Verifikacija/Rezervacija/{rezervacija.Id}&choe=UTF-8";

            string prevozIspis = "Autobuski prevoz uključen";
            if (rezervacija.Paket != null)
            {
                if (rezervacija.Paket.DostupniPrevoz == VrstaPrevoza.SamoAvion) prevozIspis = "Avionski prevoz uključen";
                else if (rezervacija.Paket.DostupniPrevoz == VrstaPrevoza.Oboje) prevozIspis = "Kombinovani prevoz (Avion/Autobus)";
            }

            var putniciRows = new System.Text.StringBuilder();
            if (rezervacija.Putnici != null && rezervacija.Putnici.Any())
            {
                int rb = 1;
                foreach (var p in rezervacija.Putnici)
                {
                    putniciRows.Append($@"
                <tr style=""border-bottom: 1px solid #e4ecee;"">
                    <td style=""padding: 12px; text-align: center; color: #536e73;"">{rb}.</td>
                    <td style=""padding: 12px; font-weight: bold; color: #0a2228;"">{p.Ime} {p.Prezime}</td>
                    <td style=""padding: 12px; color: #133a43;"">
    {(p.DatumRodjenja.HasValue ? p.DatumRodjenja.Value.ToString("dd.MM.yyyy.") : "—")}
</td>
                    <td style=""padding: 12px; color: #133a43; font-family: monospace;"">{p.BrojPasosa ?? "—"}</td>
                    <td style=""padding: 12px; color: #133a43;"">{p.Drzavljanstvo ?? "—"}</td>
                    <td style=""padding: 12px; color: #133a43;"">{p.Telefon ?? "—"}</td>
                </tr>");
                    rb++;
                }
            }
            else
            {
                putniciRows.Append(@"<tr><td colspan=""6"" style=""padding: 20px; text-align: center; color: #a4bcc0;"">Nema registrovanih putnika za ovu rezervaciju.</td></tr>");
            }

            string htmlSadrzaj = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <style>
        @page {{ size: A4; margin: 16mm; }}

        body {{
            font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
            color: #21363d;
            margin: 0;
            padding: 0;
            line-height: 1.45;
            font-size: 12px;
            background: #ffffff;
        }}

        .header-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 18px;
        }}

        .logo-title {{
            font-size: 20px;
            font-weight: 700;
            color: #16343b;
            letter-spacing: 0.5px;
        }}

        .logo-accent {{
            color: #2aa9b0;
        }}

        .doc-title {{
            text-align: right;
            font-size: 11px;
            color: #6f8a90;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
        }}

        .divider {{
            height: 2px;
            background: linear-gradient(to right, #16343b, #2aa9b0);
            margin-bottom: 18px;
        }}

        .meta-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 18px;
        }}

        .meta-box {{
            background: #f7faf9;
            padding: 12px;
            border-left: 3px solid #2aa9b0;
            border-radius: 4px;
        }}

        .meta-box h4 {{
            margin: 0 0 5px 0;
            color: #5f7c86;
            font-size: 10px;
            letter-spacing: 1px;
            text-transform: uppercase;
            font-weight: 700;
        }}

        .meta-box p {{
            margin: 0;
            font-size: 13px;
            font-weight: 700;
            color: #16343b;
        }}

        .section-title {{
            font-size: 13px;
            font-weight: 700;
            color: #16343b;
            border-bottom: 1.5px solid #2aa9b0;
            padding-bottom: 5px;
            margin-top: 18px;
            margin-bottom: 12px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}

        .grid-table {{
            width: 100%;
            border-collapse: separate;
            border-spacing: 10px;
            margin-left: -10px;
            margin-right: -10px;
        }}

        .info-card {{
            background: #ffffff;
            border: 1px solid #e3ecee;
            border-radius: 6px;
            padding: 11px 14px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.03);
        }}

        .info-card-label {{
            font-size: 9.5px;
            color: #7a9297;
            text-transform: uppercase;
            font-weight: 700;
            letter-spacing: 0.4px;
            margin-bottom: 4px;
        }}

        .info-card-value {{
            font-size: 12.5px;
            color: #16343b;
            font-weight: 600;
        }}

        .passengers-table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
            border-radius: 6px;
            overflow: hidden;
        }}

        .passengers-table th {{
            background-color: #16343b;
            color: #ffffff;
            padding: 10px;
            font-size: 10px;
            text-transform: uppercase;
            font-weight: 700;
            text-align: left;
            letter-spacing: 0.5px;
        }}

        .passengers-table td {{
            padding: 10px;
            border-bottom: 1px solid #e7eff1;
            font-size: 11px;
            color: #274148;
            vertical-align: top;
        }}

        .footer {{
            margin-top: 28px;
            text-align: center;
            font-size: 10px;
            color: #93a7ab;
            border-top: 1px solid #e4ecee;
            padding-top: 12px;
        }}
    </style>
</head>
<body>

    <table class=""header-table"">
        <tr>
            <td class=""logo-title"">Inter<span class=""logo-accent"">Trips</span></td>
            <td class=""doc-title"">Potvrda rezervacije i itinerer</td>
        
             <td class=""right-column"">
                    <img src=""{qrCodeUrl}"" width=""72"" height=""72"" style=""border: 1px solid #e4ecee; padding: 2px;"" alt=""QR Code Verification""/>
                </td>
</tr>
    </table>

    <div class=""divider""></div>

    <table class=""meta-table"">
        <tr>
            <td style=""width: 33%; padding-right: 8px;"">
                <div class=""meta-box"">
                    <h4>Broj rezervacije</h4>
                    <p>{resCode}</p>
                </div>
            </td>
            <td style=""width: 33%; padding-right: 8px;"">
                <div class=""meta-box"">
                    <h4>Datum izdavanja</h4>
                    <p>{ispravanDatum}</p>
                </div>
            </td>
            <td style=""width: 34%;"">
                <div class=""meta-box"" style=""border-left-color: {(placanje != null ? "#10b981" : "#f59e0b")};"">
                    <h4>Status aranžmana</h4>
                    <p style=""color: {(placanje != null ? "#10b981" : "#f59e0b")};"">{statusPlacanja}</p>
                </div>
            </td>
        </tr>
    </table>

    <div class=""section-title"">Detalji planiranog putovanja</div>

    <table class=""grid-table"">
        <tr>
            <td style=""width: 50%;"">
                <div class=""info-card"">
                    <div class=""info-card-label"">Odabrana destinacija / paket</div>
                    <div class=""info-card-value"">{nazivPaketa}</div>
                </div>
            </td>
            <td style=""width: 50%;"">
                <div class=""info-card"">
                    <div class=""info-card-label"">Period putovanja</div>
                    <div class=""info-card-value"">{periodPutovanja}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class=""info-card"">
                    <div class=""info-card-label"">Smještajni objekat</div>
                    <div class=""info-card-value"">{nazivHotela}</div>
                </div>
            </td>
            <td>
                <div class=""info-card"">
                    <div class=""info-card-label"">Planirani prevoz</div>
                    <div class=""info-card-value"">{prevozIspis}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class=""info-card"">
                    <div class=""info-card-label"">Ukupan broj prijavljenih putnika</div>
                    <div class=""info-card-value"">{brojPutnika} putnika</div>
                </div>
            </td>
            <td>
                <div class=""info-card"" style=""background: #f8fbfc;"">
                    <div class=""info-card-label"">Finansijski saldo</div>
                    <div class=""info-card-value"" style=""color: #1e7f88; font-size: 13px;"">{ukupnaCijena}</div>
                </div>
            </td>
        </tr>
    </table>

    <div class=""section-title"">Manifest / Podaci o putnicima</div>

    <table class=""passengers-table"">
        <thead>
            <tr>
                <th style=""width: 5%; text-align: center;"">#</th>
                <th style=""width: 30%;"">Ime i prezime</th>
                <th style=""width: 20%;"">Datum rođenja</th>
                <th style=""width: 15%;"">Broj pasoša</th>
                <th style=""width: 15%;"">Državljanstvo</th>
                <th style=""width: 15%;"">Kontakt telefon</th>
            </tr>
        </thead>
        <tbody>
            {putniciRows}
        </tbody>
    </table>

    <div class=""footer"">
        <p>Hvala Vam na povjerenju! InterTrips Turistička Agencija d.o.o.<br />
        Ovaj dokument je automatski generisan i punovažno je dokazno sredstvo o izvršenoj rezervaciji bez pečata.</p>
    </div>

</body>
</html>";

            var emailService = HttpContext.RequestServices.GetRequiredService<EmailAndDocumentService>();
            byte[] pdfBytes = emailService.GenerisiPdfDokument($"POTVRDA_{resCode}", rezervacija);

            return File(pdfBytes, "application/pdf", $"Potvrda_InterTrips_{resCode}.pdf");
        }
    }
}