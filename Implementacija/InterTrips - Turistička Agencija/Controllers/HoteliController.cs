using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Models;
using InterTrips___Turistička_Agencija.Data;

namespace InterTrips___Turistička_Agencija.Controllers
{
    public class HoteliController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoteliController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
        
            return View();
        }

    
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel)
        {
        
            if (ModelState.IsValid)
            {
               
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(hotel);
        }
    }
}
