using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class PaketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaketController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> TopDestinacije()
        {
            var topPaketi = await _db.Paketi
                .Include(p => p.Destinacija)
                .OrderByDescending(p => p.Rezervacije.Count) 
                .Take(10)
                .ToListAsync();

            foreach (var paket in topPaketi)
            {
                paket.CijenaOd = IzracunajDinamickuCijenu(paket);
            }

            return View(topPaketi);
        }

        public async Task<IActionResult> Details(int id)
        {
            var paket = await _db.Paketi
                .Include(p => p.Destinacija)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paket == null) return NotFound();

            paket.CijenaOd = IzracunajDinamickuCijenu(paket);

            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    paket.BrojPregleda += 1;
                    _db.Entry(paket).State = EntityState.Modified;

                    await _db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    
                }
            });

            return View(paket);
        }

        private decimal IzracunajDinamickuCijenu(Paket paket)
        {
            decimal baznaCijena = paket.CijenaOd;
            DateTime danas = DateTime.Now;

            double preostaloDana = (paket.DatumPolaska - danas).TotalDays;

            if (preostaloDana <= 7 && preostaloDana > 0)
            {
                baznaCijena *= 0.85m; 
            }

            

        int popunjenoMjesta = _db.Rezervacije.Count(r => r.PaketId == paket.Id);
            int slobodnoMjesta = paket.Kapacitet - popunjenoMjesta;

            if (slobodnoMjesta <= 3 && slobodnoMjesta > 0)
            {
                baznaCijena *= 1.10m;
            }

            return Math.Round(baznaCijena, 2);
        }
    }
}