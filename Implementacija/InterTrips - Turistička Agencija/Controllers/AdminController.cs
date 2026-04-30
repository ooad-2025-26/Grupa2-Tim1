using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("")]
    public IActionResult Dashboard()
        => View("~/Views/Administrator/Admin-dashboard.cshtml");
}