using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Enums;
using InterTrips___Turistička_Agencija.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers;
public class AgentController : Controller
{
    private readonly ApplicationDbContext _db;
    public AgentController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("/Agent")]
    public async Task<IActionResult> Index(int agentId)
    {
        var sviPaketi = await _db.Paketi.Include(p => p.Destinacija).ToListAsync();

        var mojiIds = await _db.AgentPaketi
            .Where(x => x.AgentId == agentId)
            .Select(x => x.PaketId)
            .ToListAsync();

        var aktivneRez = await _db.Rezervacije
            .Include(r => r.Paket)
            .ThenInclude(p => p.Destinacija)
            .Where(r => mojiIds.Contains(r.PaketId) &&
                        (r.Status == StatusRezervacije.Kreirana || r.Status == StatusRezervacije.Potvrdjena))
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var vm = new AgentDashboardVm
        {
            AgentId = agentId,
            SviPaketi = sviPaketi,
            MojiPaketiIds = mojiIds.ToHashSet(),
            AktivneRezervacije = aktivneRez
        };

        return View("~/Views/Agent/Index.cshtml", vm);
    }

    // LISTA PAKETA + odabir koje agent vodi
    [HttpGet("/Agent/Paketi")]
    public async Task<IActionResult> Paketi(int agentId)
    {
        ViewBag.AgentId = agentId;

        var sviPaketi = await _db.Paketi
            .Include(p => p.Destinacija)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        var mojiIds = await _db.AgentPaketi
            .Where(x => x.AgentId == agentId)
            .Select(x => x.PaketId)
            .ToListAsync();

        ViewBag.MojiPaketiIds = mojiIds.ToHashSet();
        return View("~/Views/Agent/Paketi.cshtml", sviPaketi);
    }

    [HttpPost("/Agent/TogglePaket")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePaket(int agentId, int paketId)
    {
        var link = await _db.AgentPaketi
            .FirstOrDefaultAsync(x => x.AgentId == agentId && x.PaketId == paketId);

        if (link == null)
            _db.AgentPaketi.Add(new AgentPaket { AgentId = agentId, PaketId = paketId });
        else
            _db.AgentPaketi.Remove(link);

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Paketi), new { agentId });
    }

    // REZERVACIJE za pakete koje agent vodi
    [HttpGet("/Agent/Rezervacije")]
    public async Task<IActionResult> Rezervacije(int agentId)
    {
        ViewBag.AgentId = agentId;

        var mojiIds = await _db.AgentPaketi
            .Where(x => x.AgentId == agentId)
            .Select(x => x.PaketId)
            .ToListAsync();

        var aktivne = await _db.Rezervacije
            .Include(r => r.Paket)
            .ThenInclude(p => p.Destinacija)
            .Where(r => mojiIds.Contains(r.PaketId) &&
                        (r.Status == StatusRezervacije.Kreirana || r.Status == StatusRezervacije.Potvrdjena))
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        return View("~/Views/Agent/Rezervacije.cshtml", aktivne);
    }
}