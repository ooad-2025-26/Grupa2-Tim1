using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("destinacije")]
    public class DestinacijeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index() => View("Destinacije");

        [HttpGet("{id:int}")]
        public IActionResult Details(int id) => View("Details");

        [HttpGet("create")]
        public IActionResult Create() => View("Create");

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost() => RedirectToAction(nameof(Index));

        [HttpGet("edit/{id:int}")]
        public IActionResult Edit(int id) => View("Edit");

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id) => RedirectToAction(nameof(Index));

        [HttpGet("delete/{id:int}")]
        public IActionResult Delete(int id) => View("Delete");

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id) => RedirectToAction(nameof(Index));
    }
}