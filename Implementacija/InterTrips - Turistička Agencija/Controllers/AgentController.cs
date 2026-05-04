using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("agent")]
    public class AgentController : Controller
    {
        [HttpGet("")]
        public IActionResult Dashboard()
            => View("~/Views/Agent/Agent-dashboard.cshtml");
    }
}