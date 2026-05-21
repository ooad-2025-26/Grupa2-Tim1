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
            var sviPaketi = await _db.Paketi
               .Include(p => p.Destinacija)
               .OrderByDescending(p => p.Id)
               .ToListAsync();

            if (sviPaketi == null) sviPaketi = new List<Paket>();

            var istaknuteDestinacije = sviPaketi.Where(p =>
            {
                var prop = p.GetType().GetProperties().FirstOrDefault(pr => pr.Name.Contains("Status"));
                if (prop != null)
                {
                    var vrijednost = prop.GetValue(p);
                    if (vrijednost != null)
                    {
                        return vrijednost.ToString() == "Dostupan" || Convert.ToInt32(vrijednost) == 0;
                    }
                }
                return true; 
            })
            .Take(10)
            .ToList();

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