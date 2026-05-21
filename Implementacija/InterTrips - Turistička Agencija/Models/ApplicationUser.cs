using Microsoft.AspNetCore.Identity;

namespace InterTrips___Turistička_Agencija.Models;

public class ApplicationUser : IdentityUser
{
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public int Uloga { get; set; }  
}