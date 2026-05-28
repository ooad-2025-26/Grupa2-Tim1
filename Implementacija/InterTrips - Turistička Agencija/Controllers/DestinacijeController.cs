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
        var sviPaketi = await _db.Paketi
            .Include(p => p.Destinacija)
            .Include(p => p.Rezervacije)
            .ToListAsync();

       var sortiraniZaTop10 = sviPaketi
            .Select(p => new {
                Paket = p,
                BrojRezervacija = p.Rezervacije?.Count ?? 0,
                Ocjena = 4.8, 
                Score = ((p.Rezervacije?.Count ?? 0) * 0.7) + (4.8 * 0.3)
            })
            .OrderByDescending(x => x.Score)
            .Take(10)
            .Select(x => x.Paket)
            .ToList();

        ViewBag.OoadTop10 = sortiraniZaTop10;

        return View(sviPaketi);
    }
}