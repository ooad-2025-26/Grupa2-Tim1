using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;

public class PlacanjeController : Controller
{
    public IActionResult Index()
    {
        return View("Placanje");
    }

    public IActionResult Uspjeh()
    {
        return View();
    }

    public IActionResult Neuspjeh(string razlog)
    {
        ViewBag.Razlog = razlog;
        return View();
    }
}