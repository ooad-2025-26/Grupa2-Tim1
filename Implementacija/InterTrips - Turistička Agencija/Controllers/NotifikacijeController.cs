using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Authorize]
    public class NotifikacijeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NotifikacijeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Moje()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var notifikacije = await _db.LogNotifikacija
                .Where(n => n.EmailPrimaoca == email)
                .OrderByDescending(n => n.VrijemeSlanja)
                .Take(50)
                .ToListAsync();

            return View("~/Views/Notifikacije/Moje.cshtml", notifikacije);
        }

        [HttpGet]
        public async Task<IActionResult> BrojNeprocitanih()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { broj = 0 });

            var broj = await _db.LogNotifikacija
                .CountAsync(n => n.EmailPrimaoca == email && !n.Procitana);

            return Json(new { broj });
        }

        [HttpPost]
        public async Task<IActionResult> OznaciSveKaoProcitane()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var neprocitane = await _db.LogNotifikacija
                .Where(n => n.EmailPrimaoca == email && !n.Procitana)
                .ToListAsync();

            foreach (var n in neprocitane)
            {
                n.Procitana = true;
                n.DatumProcitano = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Notifikacije označene kao pročitane." });
        }
    }
}