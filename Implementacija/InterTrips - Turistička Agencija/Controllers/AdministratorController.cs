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
}