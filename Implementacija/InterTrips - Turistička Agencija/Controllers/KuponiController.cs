using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterTrips___Turistička_Agencija.Data; 
using System;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class KuponiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KuponiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ProvjeriKupon(string kod)
        {
            if (string.IsNullOrEmpty(kod))
            {
                return Json(new { validan = false, poruka = "Kod kupona je prazan." });
            }

            var kupon = await _context.Kupon
                .FirstOrDefaultAsync(k => k.Kod.ToUpper() == kod.ToUpper());

            if (kupon == null)
            {
                return Json(new { validan = false, poruka = "Uneseni kupon ne postoji." });
            }

            if (kupon.Iskoristen)
            {
                return Json(new { validan = false, poruka = "Ovaj kupon je već iskorišten." });
            }

            if (kupon.VaziDo < DateTime.Now)
            {
                return Json(new { validan = false, poruka = "Rok trajanja ovog kupona je istekao." });
            }

            return Json(new
            {
                validan = true,
                popustProcenat = kupon.PopustProcenat
            });
        }
    }
}