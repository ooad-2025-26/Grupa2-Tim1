using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("korisnik")]
    public class KorisnikController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("~/Views/Korisnik/Login.cshtml");
        }

        [HttpGet("profil")]
        public IActionResult Profil()
        {
            return View("~/Views/Korisnik/Profil.cshtml");
        }

        [HttpPost("profil")]
        [ValidateAntiForgeryToken]
        public IActionResult ProfilSave()
        {
            return RedirectToAction(nameof(Profil));
        }
    }
}