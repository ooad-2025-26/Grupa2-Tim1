using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Enums;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace InterTrips___Turistička_Agencija.Controllers;

    public class AgentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AgentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var emailAgenta = User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAgenta))
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByEmailAsync(emailAgenta);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Agent"))
            {
                return Forbid();
            }

            var agentIzBaze = await _db.Set<Korisnik>().FirstOrDefaultAsync(k => k.Email == emailAgenta);
            int agentId = agentIzBaze?.Id ?? 0;

            var mojiPaketiList = await _db.Set<AgentPaket>()
                .Where(ap => ap.AgentId == agentId)
                .Select(ap => ap.PaketId)
                .ToListAsync();

            var mojiPaketiIds = new HashSet<int>(mojiPaketiList);

            var vm = new AgentDashboardVm
            {
                AgentId = agentId,
                MojiPaketiIds = mojiPaketiIds,
                SviPaketi = await _db.Paketi.Take(8).ToListAsync(),
                AktivneRezervacije = await _db.Rezervacije
                    .Include(r => r.Korisnik)
                    .Include(r => r.Paket)
                    .Where(r => mojiPaketiIds.Contains(r.PaketId))
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Rezervacije(int agentId)
        {
            var mojiPaketiIds = await _db.Set<AgentPaket>()
                .Where(ap => ap.AgentId == agentId)
                .Select(ap => ap.PaketId)
                .ToListAsync();

            var vm = new AgentDashboardVm
            {
                AgentId = agentId,
                AktivneRezervacije = await _db.Rezervacije
                    .Include(r => r.Korisnik)
                    .Include(r => r.Paket)
                        .ThenInclude(p => p != null ? p.Destinacija : null!) 
                    .Include(r => r.Putnici)
                    .Where(r => mojiPaketiIds.Contains(r.PaketId))
                    .ToListAsync()
            };

            return View("Rezervacije", vm);
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePaket(int agentId, int paketId)
    {
        var postojecaVeza = await _db.Set<AgentPaket>()
            .FirstOrDefaultAsync(ap => ap.AgentId == agentId && ap.PaketId == paketId);

        if (postojecaVeza != null)
        {
            _db.Set<AgentPaket>().Remove(postojecaVeza);
        }
        else
        {
            var noviPaket = await _db.Set<Paket>().FindAsync(paketId);
            if (noviPaket == null) return NotFound();

            var mojiTrenutniPaketi = await _db.Set<AgentPaket>()
                .Where(ap => ap.AgentId == agentId)
                .Select(ap => ap.Paket) 
                .ToListAsync();

           
            foreach (var postojeci in mojiTrenutniPaketi)
            {
                if (noviPaket.DatumPolaska < postojeci.DatumPovratka &&
                    noviPaket.DatumPovratka > postojeci.DatumPolaska)
                {
                    TempData["ErrorPoruka"] = $"Greška! Paket '{noviPaket.Naziv}' se preklapa sa " +
                        $"paketom '{postojeci.Naziv}' koji ste već preuzeli " +
                        $"({postojeci.DatumPolaska:dd.MM.yyyy} - {postojeci.DatumPovratka:dd.MM.yyyy}).";

                    return RedirectToAction("Paketi", new { agentId = agentId });
                }
            }

            var novaVeza = new AgentPaket { AgentId = agentId, PaketId = paketId };
            await _db.Set<AgentPaket>().AddAsync(novaVeza);
        }

        await _db.SaveChangesAsync();

        return RedirectToAction("Paketi", new { agentId = agentId });
    }
    [HttpGet]
        public async Task<IActionResult> Detalji(int id)
        {
            var rezervacija = await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Paket)
                    .ThenInclude(p => p != null ? p.Destinacija : null!) 
                .Include(r => r.Putnici)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rezervacija == null)
                return NotFound();

            return View(rezervacija);
        }

        [HttpGet]
        public async Task<IActionResult> Paketi(int agentId)
        {
            var mojiPaketiIds = await _db.Set<AgentPaket>()
                                          .Where(ap => ap.AgentId == agentId)
                                          .Select(ap => ap.PaketId)
                                          .ToListAsync();

            var viewModel = new AgentPaketiVm
            {
                AgentId = agentId,
                SviPaketi = await _db.Paketi.Include(p => p.Destinacija).ToListAsync(),
                MojiPaketiIds = mojiPaketiIds
            };

            return View(viewModel);
        }
    [HttpGet]
    public async Task<IActionResult> Izvjestaj(int agentId, DateTime? datumOd, DateTime? datumDo, string status)
    {
        if (agentId == 0)
        {
            var emailAgenta = User.Identity?.Name;
            if (!string.IsNullOrEmpty(emailAgenta))
            {
                var agentIzBaze = await _db.Set<Korisnik>().FirstOrDefaultAsync(k => k.Email == emailAgenta);
                agentId = agentIzBaze?.Id ?? 0;
            }
        }

        var mojiPaketiList = await _db.Set<AgentPaket>()
         .Where(ap => ap.AgentId == agentId)
         .Select(ap => ap.PaketId)
         .ToListAsync();

        var query = _db.Rezervacije
        .Include(r => r.Korisnik)
        .Include(r => r.Paket)
        .Where(r => mojiPaketiList.Contains(r.PaketId));
        if (datumOd.HasValue)
        {
            var dateOnlyOd = DateOnly.FromDateTime(datumOd.Value);
            query = query.Where(r => r.DatumPolaska >= dateOnlyOd);
        }
        if (datumDo.HasValue)
        {
            var dateOnlyDo = DateOnly.FromDateTime(datumDo.Value);
            query = query.Where(r => r.DatumPolaska <= dateOnlyDo);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status.ToString() == status);
        }

        var filtriraneRezervacije = await query.ToListAsync();

        var vm = new AgentDashboardVm
        {
            AgentId = agentId,
            MojiPaketiIds = new HashSet<int>(mojiPaketiList),
            AktivneRezervacije = filtriraneRezervacije
        };

        return View(vm);
    }
}