using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Data;
using System.Linq;
using InterTrips___Turistička_Agencija.Enums;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var istaknuteDestinacije = await _db.Paketi
                .Include(p => p.Destinacija)
                .Where(p => p.Status == StatusPaketa.Dostupan) 
                .OrderByDescending(p => p.Id)
                .Take(10)
                .ToListAsync();

            return View(istaknuteDestinacije);
        }
        public IActionResult Kontakt()
        {
            return View("~/Views/Home/Kontakt.cshtml");
        }

        public IActionResult ONama()
        {
            return View("~/Views/Home/ONama.cshtml");
        }

        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }
    }
}