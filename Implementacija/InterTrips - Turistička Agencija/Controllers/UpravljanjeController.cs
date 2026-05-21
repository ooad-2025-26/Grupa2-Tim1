using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Authorize(Roles = "Admin,Agent")] 
    public class UpravljanjeController : Controller
    {
        public IActionResult Index(string tab = "hoteli")
        {
            ViewBag.AktivniTab = tab;
            return View();
        }
    }
}