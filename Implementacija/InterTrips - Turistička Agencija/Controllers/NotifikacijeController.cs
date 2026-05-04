using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("notifikacije")]
    public class NotifikacijeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index"); 
        }
    }
}