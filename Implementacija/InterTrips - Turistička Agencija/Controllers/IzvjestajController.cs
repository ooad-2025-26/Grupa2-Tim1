using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Authorize(Roles = "Admin,Agent")] 
    public class IzvjestajController : Controller
    {
        private readonly ApplicationDbContext _db;

        public IzvjestajController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var paketi = await _db.Paketi.ToListAsync();
            var rezervacije = await _db.Rezervacije.ToListAsync();

            var topDestinacije = paketi.Select(p => new {
                Id = p.Id,
                Naziv = p.Naziv,
                Pregledi = p.BrojPregleda,
                BrojRezervacija = rezervacije.Count(r => r.PaketId == p.Id),
                StopaKonverzije = p.BrojPregleda > 0
                    ? ((double)rezervacije.Count(r => r.PaketId == p.Id) / p.BrojPregleda) * 100
                    : 0,
                Score = (rezervacije.Count(r => r.PaketId == p.Id) * 0.7) + (p.Ocjena * 0.3)
            })
            .OrderByDescending(x => x.Score)
            .Take(10) 
            .ToList();

            ViewBag.TopDestinacije = topDestinacije;

            return View();
        }
    }
}