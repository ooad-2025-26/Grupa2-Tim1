using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterTrips___Turistička_Agencija.Enums;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class AgentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AgentController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var emailAgenta = User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAgenta)) return Unauthorized("Niste ulogovani.");

            var agent = await _db.Set<Korisnik>().FirstOrDefaultAsync(k => k.Email == emailAgenta);
            if (agent == null || agent.Uloga != Uloga.Agent)
                return Forbid("Pristup dozvoljen samo turističkim agentima.");
            int agentId = agent.Id; 

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
                        .ThenInclude(p => p.Destinacija)
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
                var novaVeza = new AgentPaket { AgentId = agentId, PaketId = paketId };
                await _db.Set<AgentPaket>().AddAsync(novaVeza);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { agentId = agentId });
        }

        [HttpGet]
        public async Task<IActionResult> Detalji(int id)
        {
            var rezervacija = await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Paket)
                    .ThenInclude(p => p.Destinacija)
                .Include(r => r.Putnici)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rezervacija == null) return NotFound();

            return View(rezervacija);
        }
    }
}