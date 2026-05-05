using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class RezervacijaController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Rezervacija/Index.cshtml");
        }

        [HttpGet]
        public IActionResult Rezervacija() => View("~/Views/Rezervacija/Rezervacija.cshtml");

        [HttpGet]
        public IActionResult Putnici()
        {
            return View("~/Views/Rezervacija/Putnici.cshtml");
        }
    }
}