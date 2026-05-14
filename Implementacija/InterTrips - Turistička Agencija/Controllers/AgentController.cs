using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Enums;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace InterTrips___Turistička_Agencija.Controllers;

[Authorize(Roles = "Agent")]
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
    var mojiIds = await _db.AgentPaketi
        .Where(x => x.AgentId == agentId)
        .Select(x => x.PaketId)
        .ToListAsync();

    var aktivneRez = await _db.Rezervacije
        .Include(r => r.Paket).ThenInclude(p => p.Destinacija)
        .Where(r => mojiIds.Contains(r.PaketId) &&
                    (r.Status == StatusRezervacije.Kreirana || r.Status == StatusRezervacije.Potvrdjena))
        .OrderByDescending(r => r.Id)
        .ToListAsync();

    var vm = new AgentDashboardVm
    {
        AgentId = agentId,
        SviPaketi = await _db.Paketi.Include(p => p.Destinacija).ToListAsync(),
        MojiPaketiIds = mojiIds.ToHashSet(),
        AktivneRezervacije = aktivneRez
    };

    return View("~/Views/Agent/Index.cshtml", vm);
}
[HttpGet("/Agent/Rezervacije")]
public async Task<IActionResult> Rezervacije(int agentId)
{
    var rezervacije = await _db.Rezervacije
        .Include(r => r.Paket).ThenInclude(p => p.Destinacija)
        .Include(r => r.Korisnik)
        .OrderByDescending(r => r.Korisnik != null && r.Korisnik.Email == "alex@example.com")
        .ThenByDescending(r => r.Id)
        .ToListAsync();

    var vm = new AgentDashboardVm
    {
        AgentId = agentId,
        AktivneRezervacije = rezervacije,
        SviPaketi = new List<Paket>(),
        MojiPaketiIds = new HashSet<int>()
    };

    return View("~/Views/Agent/Rezervacije.cshtml", vm);
}

[HttpGet("/Agent/Paketi")]
public async Task<IActionResult> Paketi(int agentId)
{
    var vm = new AgentPaketiVm
    {
        AgentId = agentId,
        SviPaketi = await _db.Paketi.Include(p => p.Destinacija).ToListAsync(),
        MojiPaketiIds = await _db.AgentPaketi
            .Where(ap => ap.AgentId == agentId)
            .Select(ap => ap.PaketId)
            .ToListAsync()
    };

    return View("~/Views/Agent/Paketi.cshtml", vm);
}

[HttpPost("/Agent/TogglePaket")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> TogglePaket(int agentId, int paketId)
{
    if (agentId <= 0)
    {
        return BadRequest("Greška: Nevalidan ID agenta. Provjerite da li ste prijavljeni.");
    }

    var agentPostoji = await _db.Korisnici.AnyAsync(u => u.Id == agentId);
    if (!agentPostoji)
    {
        return NotFound($"Greška: Agent sa ID-om {agentId} ne postoji u bazi podataka.");
    }

    var postojeci = await _db.AgentPaketi
        .FirstOrDefaultAsync(ap => ap.AgentId == agentId && ap.PaketId == paketId);
    System.Diagnostics.Debug.WriteLine("DEBUG: Primljeni AgentId je " + agentId);
    System.Diagnostics.Debug.WriteLine("DEBUG: Primljeni PaketId je " + paketId);
    if (postojeci != null)
    {
        _db.AgentPaketi.Remove(postojeci);
    }
    else
    {
        _db.AgentPaketi.Add(new AgentPaket { AgentId = agentId, PaketId = paketId });
    }

    try
    {
        await _db.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        return BadRequest("Došlo je do greške prilikom spašavanja u bazu: " + ex.Message);
    }

    return Redirect(Request.Headers["Referer"].ToString());
}
}