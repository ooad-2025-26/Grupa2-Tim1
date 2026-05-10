using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _db; 

        public RezervacijaController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Rezervacija/Index.cshtml");
        }

        [HttpGet]
        public IActionResult Rezervacija()
        {
            var paketi = _db.Paketi
                                 .Include(p => p.Destinacija)
                                 .ToList();

            if (paketi == null) paketi = new List<InterTrips___Turistička_Agencija.Models.Paket>();

            return View("~/Views/Rezervacija/Rezervacija.cshtml", paketi);
        }

        [HttpGet]
        public IActionResult Putnici()
        {
            return View("~/Views/Rezervacija/Putnici.cshtml");
        }
    }
}