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
            KorisniciCount = _userManager.Users.Count(),
            RezervacijeCount = await _db.Rezervacije.CountAsync(),
            Destinacije = await _db.Destinacije.OrderByDescending(d => d.Id).ToListAsync()
        };

        return View("~/Views/Administrator/AdminDashboard.cshtml", vm);
    }
    [HttpGet]
    public async Task<IActionResult> Destinacije()
    {
        var list = await _db.Destinacije
            .OrderBy(d => d.Id)
            .ToListAsync();

        return View("~/Views/Administrator/Destinacije.cshtml", list);
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
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return View("~/Views/Administrator/Paketi.cshtml", list);
    }

    [HttpGet]
    public async Task<IActionResult> PaketCreate(int? destinacijaId)
    {
        ViewBag.Destinacije = await _db.Destinacije
            .OrderBy(d => d.Naziv)
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.Naziv} ({d.Drzava})" })
            .ToListAsync();

        ViewBag.Statusi = Enum.GetValues(typeof(StatusPaketa))
            .Cast<StatusPaketa>()
            .Select(s => new SelectListItem { Value = ((int)s).ToString(), Text = s.ToString() })
            .ToList();

        var paket = new Paket();
        if (destinacijaId.HasValue)
            paket.DestinacijaId = destinacijaId.Value;

        return View("~/Views/Administrator/PaketForm.cshtml", paket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaketCreate(Paket model)
    {
        if (!ModelState.IsValid)
        {
            await FillPaketDropdowns(model.DestinacijaId, model.Status);
            return View("~/Views/Administrator/PaketForm.cshtml", model);
        }

        _db.Paketi.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Paketi));
    }

    [HttpGet]
    public async Task<IActionResult> PaketEdit(int id)
    {
        var p = await _db.Paketi.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();

        await FillPaketDropdowns(p.DestinacijaId, p.Status);
        return View("~/Views/Administrator/PaketForm.cshtml", p);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaketEdit(int id, Paket model)
    {
        var p = await _db.Paketi.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();

        if (!ModelState.IsValid)
        {
            await FillPaketDropdowns(model.DestinacijaId, model.Status);
            return View("~/Views/Administrator/PaketForm.cshtml", model);
        }

        p.Naziv = model.Naziv;
        p.CijenaOd = model.CijenaOd;
        p.TrajanjeNoci = model.TrajanjeNoci;
        p.Status = model.Status;
        p.DestinacijaId = model.DestinacijaId;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Paketi));
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

    [Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> PlanTemplateDodajStavku(int planId, int redniBroj, string naziv, string? opis, TimeSpan? vrijeme)
    {
        var plan = await _db.PlanoviPutovanjaTemplate.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null) return NotFound();

        _db.StavkePlanaTemplate.Add(new StavkaPlanaTemplate
        {
            PlanPutovanjaTemplateId = planId,
            RedniBroj = redniBroj,
            Naziv = naziv,
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

    [HttpGet("Administrator/Rezervacije")]
    public async Task<IActionResult> Rezervacije()
    {
        var ciljaniEmail = "test@intertrips.ba";

         var sveRezervacije = await _db.Rezervacije
            .Include(r => r.Paket)
                .ThenInclude(p => p!.Destinacija)
            .Include(r => r.Korisnik)
            .Include(r => r.Putnici)
            .Include(r => r.Placanje)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var viewModel = new AdminRezervacijeVm
        {
            KorisnikEmail = ciljaniEmail,
            Rezervacije = sveRezervacije 
        };

        return View(viewModel);
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

        return View(viewModel);
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
}