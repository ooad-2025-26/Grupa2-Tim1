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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessInquiry(string name, string email, string topic, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Sva polja su obavezna!" });
            }

            var noviUpit = new KontaktUpit
            {
                Ime = name,
                Email = email,
                Tema = topic,
                Poruka = message
            };

            _db.KontaktUpit.Add(noviUpit);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Vaš upit je uspješno zaprimljen!" });
        }

       
    }
}