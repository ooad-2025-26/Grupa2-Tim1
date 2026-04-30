using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;

[Route("paketi")]
public class PaketiController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("Index"); 

    [HttpGet("{id:int}")]
    public IActionResult Details(int id) => View("Details"); 

    
}