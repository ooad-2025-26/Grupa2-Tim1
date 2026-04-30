using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("rezervacija")]
    public class RezervacijaController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Rezervacija"); 
        }

        [HttpGet("putnici")]
        public IActionResult Putnici()
        {
            return View("Putnici"); 
        }
    }
}