using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RezervacijaController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Rezervacija/Index.cshtml");
        }

        [HttpGet]
        public IActionResult Rezervacija()
        {
            var paketi = _db.Paketi
                                 .Include(p => p.Destinacija)
                                 .ToList();

            if (paketi == null) paketi = new List<Paket>();

            return View("~/Views/Rezervacija/Rezervacija.cshtml", paketi);
        }

        [HttpGet]
        public IActionResult Putnici()
        {
            return View("~/Views/Rezervacija/Putnici.cshtml");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> KreirajRezervaciju([FromBody] NovaRezervacijaDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? emailKorisnika = User.Identity?.Name;
            if (string.IsNullOrEmpty(emailKorisnika)) return Unauthorized("Korisnik nije ulogovan.");

            var korisnik = _db.Users.FirstOrDefault(u => u.Email == emailKorisnika);
            if (korisnik == null) return Unauthorized("Korisnik nije pronađen u Identity bazi podataka.");

            var paket = await _db.Paketi.FirstOrDefaultAsync(p => p.Id == model.PaketId);
            if (paket == null)
            {
                return BadRequest($"Odabrani PaketId ({model.PaketId}) ne postoji u bazi podataka.");
            }

            var stvarniKorisnikUBazi = _db.Korisnici.FirstOrDefault(k => k.Email == emailKorisnika);
            if (stvarniKorisnikUBazi == null)
            {
                return BadRequest("Korisnik nema kreiran profil u tabeli Korisnici.");
            }

            decimal baznaCijena = 500; 

            var propertyCijena = paket.GetType().GetProperties().FirstOrDefault(p => p.Name.Contains("Cijena") || p.Name.Contains("Price"));
            if (propertyCijena != null)
            {
                var vrijednost = propertyCijena.GetValue(paket);
                if (vrijednost != null) baznaCijena = Convert.ToDecimal(vrijednost);
            }

            decimal konacnaCijena = baznaCijena * model.Putnici.Count;

            if (!string.IsNullOrEmpty(model.PromoKod))
            {
                if (model.PromoKod.ToUpper() == "LETO2026" || model.PromoKod.ToUpper() == "INTERTRIPS10")
                {
                    decimal popust = konacnaCijena * 0.15m; 
                    konacnaCijena -= popust;
                }
                else
                {
                    return BadRequest("Uneseni promo kod nije važeći ili je istekao.");
                }
            }

            var novaRezervacija = new Rezervacija
            {
                PaketId = model.PaketId,
                DatumPolaska = model.DatumPolaska,
                DatumPovratka = model.DatumPovratka,
                Status = StatusRezervacije.Kreirana,
                KorisnikId = stvarniKorisnikUBazi.Id,

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

            try
            {
                _db.Rezervacije.Add(novaRezervacija);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    id = novaRezervacija.Id,
                    cijenaZaPlacanje = konacnaCijena,
                    message = "Rezervacija uspješno spašena u bazu!"
                });
            }
            catch (DbUpdateException ex)
            {
                var stvarnaGreska = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"SQL Greška pri čuvanju: {stvarnaGreska}");
            }
        }

        [Authorize]
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

                var korisnik = await _db.Korisnici.FirstOrDefaultAsync(k => k.Email == emailKorisnika);

                if (korisnik == null)
                {
                    var identityUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailKorisnika);

                    if (identityUser != null)
                    {
                        var noviKorisnikProfil = new Korisnik
                        {
                            Email = emailKorisnika,
                            Ime = emailKorisnika.Split('@')[0],
                            Lozinka = "Identity_Managed",
                            Uloga = Uloga.Klijent
                        };

                        _db.Korisnici.Add(noviKorisnikProfil);
                        await _db.SaveChangesAsync();

                        korisnik = noviKorisnikProfil;
                    }
                    else
                    {
                        return NotFound(new { greska = "Korisnik ne postoji ni u Identity bazi podataka." });
                    }
                }

                var siroveRezervacije = await _db.Rezervacije
                    .Include(r => r.Paket)
                        .ThenInclude(p => p.Destinacija)
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> OtkaziRezervaciju(int id)
        {
            try
            {
                var emailKorisnika = User.Identity?.Name;
                var korisnik = await _db.Korisnici.FirstOrDefaultAsync(k => k.Email == emailKorisnika);

                if (korisnik == null) return Unauthorized("Korisnik nije pronađen.");

                var rezervacija = await _db.Rezervacije.FirstOrDefaultAsync(r => r.Id == id && r.KorisnikId == korisnik.Id);

                if (rezervacija == null) return NotFound("Rezervacija nije pronađena ili nemate pravo da je otkažete.");

                rezervacija.Status = StatusRezervacije.Otkazana;
                await _db.SaveChangesAsync();

                return Ok(new { message = "Rezervacija je uspješno otkazana." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { greska = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProvjeriKupon(string kod, int paketId)
        {
            if (string.IsNullOrEmpty(kod)) return BadRequest("Kod nije unesen.");

            var paket = await _db.Paketi.FindAsync(paketId);
            if (paket == null) return BadRequest("Odabrani paket ne postoji.");

            decimal staraCijena = 500;
            var propertyCijena = paket.GetType().GetProperties().FirstOrDefault(p => p.Name.Contains("Cijena") || p.Name.Contains("Price"));
            if (propertyCijena != null)
            {
                var vrijednost = propertyCijena.GetValue(paket);
                if (vrijednost != null) staraCijena = Convert.ToDecimal(vrijednost);
            }

            if (kod.ToUpper() == "LETO2026" || kod.ToUpper() == "INTERTRIPS10")
            {
                decimal popustIznos = staraCijena * 0.15m; 
                decimal novaCijena = staraCijena - popustIznos;

                return Ok(new
                {
                    validan = true,
                    poruka = "Kupon uspješno primijenjen! Popust: 15%",
                    novaCijena = novaCijena,
                    usteda = popustIznos
                });
            }

            return NotFound(new { validan = false, poruka = "Uneseni kupon ne postoji ili je istekao." });
        }
    }
}