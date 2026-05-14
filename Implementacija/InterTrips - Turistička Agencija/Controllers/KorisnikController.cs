using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("Korisnik")]
    public class KorisnikController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet("profil")]
        public IActionResult Profil()
        {
            return View("~/Views/Account/Profil.cshtml");
        }

        [Authorize]
        [HttpPost("profil")]
        [ValidateAntiForgeryToken]
        public IActionResult ProfilSave()
        {
            return RedirectToAction(nameof(Profil));
        }
    }
}