using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Enums;
using InterTrips___Turistička_Agencija.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InterTrips___Turistička_Agencija.Controllers;

[Authorize(Roles = "Admin")]
public class AdministratorController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdministratorController(ApplicationDbContext db, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _env = env;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new AdminDashboardVm
        {
            DestinacijeCount = await _db.Destinacije.CountAsync(),
            KorisniciCount = await _userManager.Users.CountAsync(),
            RezervacijeCount = await _db.Rezervacije.CountAsync(),
            Destinacije = await _db.Destinacije.OrderByDescending(d => d.Id).ToListAsync(),
            Letovi = await _db.Letovi.ToListAsync(),
            Hoteli = await _db.Hoteli.ToListAsync()
        };

        return View("~/Views/Administrator/AdminDashboard.cshtml", vm);
    }

    [HttpGet]
    public IActionResult Destinacije()
    {
        var sveDestinacije = _db.Destinacije.ToList();
        var brojKorisnika = _db.Korisnici.Count();
        var brojRezervacija = _db.Rezervacije.Count();

        var viewModel = new AdminDashboardVm
        {
            Destinacije = sveDestinacije,
            DestinacijeCount = sveDestinacije.Count,
            KorisniciCount = brojKorisnika,
            RezervacijeCount = brojRezervacija
        };

        return View("~/Views/Administrator/Destinacije.cshtml", viewModel);
    }

    [HttpGet]
    public IActionResult DestinacijaCreate()
    {
        return View("~/Views/Administrator/DestinacijaForm.cshtml", new Destinacija());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DestinacijaCreate(Destinacija model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Administrator/DestinacijaForm.cshtml", model);

        var file = Request.Form.Files.FirstOrDefault();
        if (file != null && file.Length > 0)
            await SaveDestinacijaImage(model, file);

        _db.Destinacije.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(PaketCreate), new { destinacijaId = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> DestinacijaEdit(int id)
    {
        var d = await _db.Destinacije.FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return NotFound();

        return View("~/Views/Administrator/DestinacijaForm.cshtml", d);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DestinacijaEdit(int id, Destinacija model)
    {
        var d = await _db.Destinacije.FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return NotFound();

        if (!ModelState.IsValid)
            return View("~/Views/Administrator/DestinacijaForm.cshtml", model);

        d.Naziv = model.Naziv;
        d.Drzava = model.Drzava;
        d.Kategorija = model.Kategorija;
        d.Opis = model.Opis;

        var file = Request.Form.Files.FirstOrDefault();
        if (file != null && file.Length > 0)
            await SaveDestinacijaImage(d, file);

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Destinacije));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DestinacijaDelete(int id)
    {
        var d = await _db.Destinacije.FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return NotFound();

        _db.Destinacije.Remove(d);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Destinacije));
    }

    private async Task SaveDestinacijaImage(Destinacija destinacija, Microsoft.AspNetCore.Http.IFormFile file)
    {
        var assetsPath = Path.Combine(_env.WebRootPath, "assets");
        Directory.CreateDirectory(assetsPath);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var fileName = $"dest_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(assetsPath, fileName);

        using var fs = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fs);

        destinacija.SlikaUrl = $"/assets/{fileName}";
    }

    [HttpGet]
    public async Task<IActionResult> Paketi()
    {
        var list = await _db.Paketi
            .Include(p => p.Destinacija)
            .Include(p => p.Rezervacije)
            .ThenInclude(r => r.Putnici)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return View("~/Views/Administrator/Paketi.cshtml", list);
    }

    [HttpGet]
    public async Task<IActionResult> PaketCreate()
    {
        ViewBag.Destinacije = await _db.Destinacije
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Naziv })
            .ToListAsync();

        ViewBag.Hoteli = await _db.Hoteli
            .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Naziv })
            .ToListAsync();

        ViewBag.Letovi = await _db.Letovi
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = $"{l.BrojLeta} ({l.Polazak} - {l.Odrediste})" })
            .ToListAsync();

        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa))
            .Cast<StatusPaketa>()
            .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

        return View("PaketForm", new Paket());
    }

    [HttpGet]
    public async Task<IActionResult> PaketEdit(int id)
    {
        var paket = await _db.Paketi.FindAsync(id);
        if (paket == null) return NotFound();

        ViewBag.Destinacije = await _db.Destinacije
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Naziv })
            .ToListAsync();

        ViewBag.Hoteli = await _db.Hoteli
            .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Naziv })
            .ToListAsync();

        ViewBag.Letovi = await _db.Letovi
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = $"{l.BrojLeta} ({l.Polazak} - {l.Odrediste})" })
            .ToListAsync();

        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa))
            .Cast<StatusPaketa>()
            .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

        return View("PaketForm", paket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaketCreate(Paket paket)
    {
        if (ModelState.IsValid)
        {
            _db.Paketi.Add(paket);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Paketi));
        }

        ViewBag.Destinacije = await _db.Destinacije
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Naziv })
            .ToListAsync();

        ViewBag.Hoteli = await _db.Hoteli
            .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Naziv })
            .ToListAsync();

        ViewBag.Letovi = await _db.Letovi
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = $"{l.BrojLeta} ({l.Polazak} - {l.Odrediste})" })
            .ToListAsync();

        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa))
            .Cast<StatusPaketa>()
            .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

        return View("PaketForm", paket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaketEdit(int id, Paket paket)
    {
        if (id != paket.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
               _db.Update(paket);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Paketi.Any(e => e.Id == paket.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Paketi));
        }
        ViewBag.Destinacije = await _db.Destinacije.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Naziv }).ToListAsync();
        ViewBag.Hoteli = await _db.Hoteli.Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Naziv }).ToListAsync();
        ViewBag.Letovi = await _db.Letovi.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.BrojLeta }).ToListAsync();
        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa)).Cast<StatusPaketa>().Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

        return View("PaketForm", paket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaketDelete(int id)
    {
        var p = await _db.Paketi.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();

        _db.Paketi.Remove(p);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Paketi));
    }

    private async Task FillPaketDropdowns(int selectedDestinacijaId, StatusPaketa selectedStatus)
    {
        ViewBag.Destinacije = await _db.Destinacije
            .OrderBy(d => d.Naziv)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Naziv} ({d.Drzava})",
                Selected = d.Id == selectedDestinacijaId
            })
            .ToListAsync();

        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa))
            .Cast<StatusPaketa>()
            .Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.ToString(),
                Selected = s == selectedStatus
            })
            .ToList();
    }

    [HttpGet]
    public async Task<IActionResult> PlanTemplate(int paketId)
    {
        var plan = await _db.PlanoviPutovanjaTemplate
            .Include(p => p.Paket)
                .ThenInclude(pk => pk!.Destinacija)
            .Include(p => p.Stavke)
            .FirstOrDefaultAsync(p => p.PaketId == paketId);

        if (plan == null)
        {
            plan = new PlanPutovanjaTemplate { PaketId = paketId };
            _db.PlanoviPutovanjaTemplate.Add(plan);
            await _db.SaveChangesAsync();

            plan = await _db.PlanoviPutovanjaTemplate
                .Include(p => p.Paket)
                    .ThenInclude(pk => pk!.Destinacija)
                .Include(p => p.Stavke)
                .FirstAsync(p => p.Id == plan.Id);
        }

        return View("~/Views/Administrator/PlanTemplate.cshtml", plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanTemplateSaveNapomena(int id, string? napomena)
    {
        var plan = await _db.PlanoviPutovanjaTemplate.FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null) return NotFound();

        plan.Napomena = napomena;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(PlanTemplate), new { paketId = plan.PaketId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanTemplateDodajStavku(int planId, int redniBroj, string Naziv, string? opis, TimeSpan? vrijeme)
    {
        var plan = await _db.PlanoviPutovanjaTemplate.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null) return NotFound();

        _db.StavkePlanaTemplate.Add(new StavkaPlanaTemplate
        {
            PlanPutovanjaTemplateId = planId,
            RedniBroj = redniBroj,
            Naziv = Naziv, 
            Opis = opis,
            Vrijeme = vrijeme
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(PlanTemplate), new { paketId = plan.PaketId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanTemplateObrisiStavku(int id)
    {
        var stavka = await _db.StavkePlanaTemplate
            .Include(s => s.PlanPutovanjaTemplate)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stavka == null) return NotFound();

        var paketId = stavka.PlanPutovanjaTemplate!.PaketId;

        _db.StavkePlanaTemplate.Remove(stavka);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(PlanTemplate), new { paketId });
    }

    [HttpGet]
    public async Task<IActionResult> Rezervacije(string? emailPretraga)
    {
        var query = _db.Rezervacije
           .Include(r => r.Paket)
           .ThenInclude(p => p!.Destinacija)
           .Include(r => r.Korisnik)
           .Include(r => r.Putnici)
           .Include(r => r.Placanje)
           .Include(r => r.Paket)
        .ThenInclude(p => p.Hotel) 
    .Include(r => r.Paket)
        .ThenInclude(p => p.Let)
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(emailPretraga))
        {
            query = query.Where(r => r.Korisnik!.Email!.Contains(emailPretraga));
        }

        var sveRezervacije = await query.OrderByDescending(r => r.Id).ToListAsync();

        var viewModel = new AdminRezervacijeVm
        {
            KorisnikEmail = emailPretraga ?? "Sve rezervacije",
            Rezervacije = sveRezervacije
        };

        return View("~/Views/Administrator/Rezervacije.cshtml", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Korisnici()
    {
        var destinacijeCount = await _db.Destinacije.CountAsync();
        var rezervacijeCount = await _db.Rezervacije.CountAsync();

        var agentiCount = await _userManager.Users.Where(u => u.Uloga == 1).CountAsync();
        var korisniciIzBaze = await _userManager.Users.ToListAsync();

        var viewModel = new AdminDashboardVm
        {
            DestinacijeCount = destinacijeCount,
            KorisniciCount = agentiCount,
            RezervacijeCount = rezervacijeCount,
            Destinacije = await _db.Destinacije.ToListAsync(),
            SviKorisnici = korisniciIzBaze
        };

        return View("~/Views/Administrator/Korisnici.cshtml", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PromijeniUlogu(string korisnikId, int novaUloga)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);

        if (korisnik != null)
        {
            korisnik.Uloga = novaUloga;
            await _userManager.UpdateAsync(korisnik);

            var trenutneUloge = await _userManager.GetRolesAsync(korisnik);
            if (trenutneUloge.Any())
            {
                await _userManager.RemoveFromRolesAsync(korisnik, trenutneUloge);
            }

            if (novaUloga == 2)
            {
                await _userManager.AddToRoleAsync(korisnik, "Admin");
            }
            else if (novaUloga == 1)
            {
                await _userManager.AddToRoleAsync(korisnik, "Agent");
            }
            else
            {
                await _userManager.AddToRoleAsync(korisnik, "Client");
            }
        }

        return RedirectToAction("Korisnici");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LetCreate(Let model)
    {
        if (!ModelState.IsValid)
        {
            var vm = new AdminDashboardVm
            {
                Destinacije = await _db.Destinacije.ToListAsync(),
                Letovi = await _db.Letovi.ToListAsync()
            };
            TempData["Error"] = "Podaci nisu validni.";
            return View("Letovi", vm);
        }

        _db.Letovi.Add(model);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Let uspješno kreiran!";
        return RedirectToAction("Letovi");
    }

    [HttpGet]
    public async Task<IActionResult> Letovi()
    {
        var viewModel = new AdminDashboardVm
        {
            DestinacijeCount = await _db.Destinacije.CountAsync(),
            KorisniciCount = await _userManager.Users.CountAsync(),
            RezervacijeCount = await _db.Rezervacije.CountAsync(),
            Destinacije = await _db.Destinacije.OrderByDescending(d => d.Id).ToListAsync(),
            Letovi = await _db.Letovi.ToListAsync(),
            Hoteli = await _db.Hoteli.ToListAsync()
        };
        return View("~/Views/Administrator/Letovi.cshtml", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Hoteli()
    {
        var hoteli = await _db.Hoteli
            .Include(h => h.Destinacija)
            .ToListAsync();

        var viewModel = new HoteliPageViewModel
        {
            Hoteli = hoteli
        };

        ViewBag.DestinacijaId = new SelectList(await _db.Destinacije.ToListAsync(), "Id", "Naziv");

        return View("~/Views/Administrator/Hoteli.cshtml", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HotelCreate(Hotel hotel)
    {
        if (ModelState.IsValid)
        {
            _db.Add(hotel);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Hotel uspješno dodan!";
            return RedirectToAction(nameof(Hoteli));
        }

        TempData["Error"] = "Greška pri dodavanju hotela. Provjerite unesene podatke.";
        return RedirectToAction(nameof(Hoteli));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HotelDelete(int id)
    {
        var hotel = await _db.Hoteli.FindAsync(id);
        if (hotel == null)
        {
            TempData["Error"] = "Hotel nije pronađen.";
            return RedirectToAction("Hoteli");
        }

        _db.Hoteli.Remove(hotel);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Hotel je uspješno obrisan.";
        return RedirectToAction("Hoteli");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetujSveRezervacije()
    {
        try
        {
            var sveRezervacije = await _db.Rezervacije.Include(r => r.Putnici).ToListAsync();

            if (sveRezervacije.Any())
            {
                _db.Rezervacije.RemoveRange(sveRezervacije);
            }

            var hoteli = await _db.Hoteli.ToListAsync();
            foreach (var hotel in hoteli)
            {
                 hotel.DostupnoSoba = 400;
            }

            var paketi = await _db.Paketi.ToListAsync();
            foreach (var paket in paketi)
            {
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Sve rezervacije su uspješno obrisane, a kapaciteti hotela su resetovani!";
        }
        catch (Exception ex)
        {
            var greska = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            TempData["Error"] = "Došlo je do greške pri resetovanju: " + greska;
        }

        return RedirectToAction("Rezervacije");
    }

    [HttpGet]
    public async Task<IActionResult> Izvjestaj()
    {
        var prometPotvrdjeno = await _db.Placanja
        .SumAsync(p => (double?)p.Iznos) ?? 0.0;

        var prometNaCekanju = await _db.Rezervacije
        .Where(r => r.Status.ToString() == "NaCekanju" && r.Paket != null)
        .SumAsync(r => (double?)r.Paket.CijenaOd) ?? 0.0;

        var ukupanPrometSve = prometPotvrdjeno + prometNaCekanju;
        ViewBag.PrometPotvrdjeno = prometPotvrdjeno;
        ViewBag.PrometNaCekanju = prometNaCekanju;
        ViewBag.UkupanPrometSve = ukupanPrometSve;

        var paketi = await _db.Paketi
            .Include(p => p.Rezervacije)
            .ToListAsync();

        var topDestinacije = paketi.Select(p => {
            double brojRezervacija = p.Rezervacije?.Count ?? 0;
            double pregledi = p.BrojPregleda > 0 ? p.BrojPregleda : (brojRezervacija + 10); 
            double stopaKonverzije = (brojRezervacija / pregledi) * 100;

            double prosjecnaOcjena = 4.5;
            double score = (brojRezervacija * 0.7) + (prosjecnaOcjena * 0.3);

            return new
            {
                p.Naziv,
                Pregledi = (int)pregledi,
                BrojRezervacija = (int)brojRezervacija,
                StopaKonverzije = stopaKonverzije,
                Score = score
            };
        })
        .OrderByDescending(d => d.Score)
        .Take(10) 
        .ToList();

        ViewBag.TopDestinacije = topDestinacije;
        System.Dynamic.ExpandoObject dummyModel = new System.Dynamic.ExpandoObject();
        return View("~/Views/Administrator/Izvjestaj.cshtml", null);
    }

    [HttpGet]
    public async Task<IActionResult> LetEdit(int id)
    {
        var let = await _db.Letovi.FindAsync(id);
        if (let == null)
        {
            TempData["Error"] = "Traženi let ne postoji.";
            return RedirectToAction("Letovi"); 
        }

        ViewBag.Destinacije = await _db.Destinacije.ToListAsync();

        return View(let);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LetEdit(Let model)
    {
        if (ModelState.IsValid)
        {
            _db.Letovi.Update(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Let je uspješno izmijenjen.";
            return RedirectToAction("Letovi"); 
        }

        ViewBag.Destinacije = await _db.Destinacije.ToListAsync();
        TempData["Error"] = "Molimo ispravite greške u formi.";
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Upiti()
    {
        var upiti = await _db.KontaktUpit.OrderByDescending(u => u.DatumSlanja).ToListAsync();

        return View("~/Views/Administrator/Upiti.cshtml", upiti);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var upit = await _db.KontaktUpit.FindAsync(id);
        if (upit == null)
        {
            return NotFound();
        }

        upit.Procitano = true;

        _db.KontaktUpit.Update(upit);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
