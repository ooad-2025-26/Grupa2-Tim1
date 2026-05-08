using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    => RedirectToAction("Index", "Administrator");
}