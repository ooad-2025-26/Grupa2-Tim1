namespace InterTrips___Turistička_Agencija.Models.ViewModels;

public class AdminDashboardVm
{
    public int DestinacijeCount { get; set; }
    public int KorisniciCount { get; set; }
    public int RezervacijeCount { get; set; }
    public List<ApplicationUser> SviKorisnici { get; set; } = new();
    public List<Destinacija> Destinacije { get; set; } = new();
    public List<Paket> Paketi { get; set; } 
    public List<Korisnik> Korisnici { get; set; }
    public List<Rezervacija> Rezervacije { get; set; } = new();
}