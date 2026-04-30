using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers

[Route("korisnik")]
public class KorisnikController : Controller
{
    [HttpGet("login")]
    public IActionResult Login()
        => View("~/Views/Korisnik/Login.cshtml");
}