using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers;

[Authorize]
[Route("PlanPutovanja")]
public class PlanPutovanjaController : Controller
{
    private readonly ApplicationDbContext _db;
    public PlanPutovanjaController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index(int rezervacijaId)
    {
        var plan = await _db.PlanoviPutovanja
            .Include(p => p.StavkePlana)
            .FirstOrDefaultAsync(p => p.RezervacijaId == rezervacijaId);

        if (plan == null)
        {
            plan = new PlanPutovanja { RezervacijaId = rezervacijaId };
            _db.PlanoviPutovanja.Add(plan);
            await _db.SaveChangesAsync();
        }

        return View(plan); 
    }

    [HttpGet("Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var plan = await _db.PlanoviPutovanja
            .Include(p => p.StavkePlana)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null) return NotFound();
        return View(plan); 
    }

    [HttpPost("DodajStavku")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DodajStavku(int planId, int redniBroj, string naziv, string? opis, DateTime? datumVrijeme)
    {
        var plan = await _db.PlanoviPutovanja.FindAsync(planId);
        if (plan == null) return NotFound();

        _db.StavkePlana.Add(new StavkaPlana
        {
            PlanPutovanjaId = planId,
            RedniBroj = redniBroj,
            Naziv = naziv,
            Opis = opis,
            DatumVrijeme = datumVrijeme
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { rezervacijaId = plan.RezervacijaId });
    }

    [HttpGet("Preview")] 
    public async Task<IActionResult> Preview(int paketId)
    {
        var plan = await _db.PlanoviPutovanjaTemplate
            .Include(p => p.Paket)
                .ThenInclude(pk => pk!.Destinacija)
            .Include(p => p.Stavke)
            .FirstOrDefaultAsync(p => p.PaketId == paketId);
        if (plan == null) return NotFound();

        return View(plan);
    }
}