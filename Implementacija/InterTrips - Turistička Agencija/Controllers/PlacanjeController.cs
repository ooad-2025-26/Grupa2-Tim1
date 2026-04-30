using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("placanje")]
    public class PlacanjeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Placanje"); 
        }
    }
}