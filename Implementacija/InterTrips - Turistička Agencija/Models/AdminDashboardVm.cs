namespace InterTrips___Turistička_Agencija.Models.ViewModels;

public class AdminDashboardVm
{
    public int DestinacijeCount { get; set; }
    public int KorisniciCount { get; set; }
    public int RezervacijeCount { get; set; }

    public List<Destinacija> Destinacije { get; set; } = new();
}