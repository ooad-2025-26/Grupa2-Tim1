using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("Korisnik")]
    public class KorisnikController : Controller
    {
        private readonly ApplicationDbContext _db;
        public KorisnikController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("~/Views/Korisnik/Login.cshtml");
        }

        [HttpPost("LoginAgent")]
        public async Task<IActionResult> LoginAgent([FromBody] LoginModel model)
        {
            var korisnik = await _db.Korisnici.FirstOrDefaultAsync(u => u.Email == model.Email && u.Lozinka == model.Lozinka);

            if (korisnik != null)
            {
                return Json(new { success = true, userId = korisnik.Id });
            }

            return Json(new { success = false });
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Lozinka { get; set; }
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