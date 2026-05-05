using InterTrips___Turistička_Agencija.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Controllers;

public class DestinacijeController : Controller
{
    private readonly ApplicationDbContext _context;

    public DestinacijeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var paketi = _context.Paketi
            .Include(p => p.Destinacija)
            .ToList();

        return View(paketi);
    }
}