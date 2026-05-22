using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers;

public class DestinacijeController : Controller
{
    private readonly ApplicationDbContext _db;
    public DestinacijeController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var paketi = await _db.Paketi
                          .Include(p => p.Destinacija)
                          .Include(p => p.Hotel)
                          .ToListAsync();
        return View(paketi);
    }
}