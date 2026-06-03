using InterTrips___Turistička_Agencija.Models;
using System.Collections.Generic;

namespace InterTrips___Turistička_Agencija.Models.ViewModels
{
    public class AdminDashboardVm
    {
        public int? DestinacijaId { get; set; }
        public string? Aviokompanija { get; set; } 

        public int DestinacijeCount { get; set; }
        public int KorisniciCount { get; set; }
        public int RezervacijeCount { get; set; }

        public List<ApplicationUser> SviKorisnici { get; set; } = new();
        public List<Korisnik> Korisnici { get; set; } = new();
        public List<Destinacija> Destinacije { get; set; } = new();
        public List<Paket> Paketi { get; set; } = new();
        public List<Rezervacija> Rezervacije { get; set; } = new();
        public List<Let> Letovi { get; set; } = new();
        public List<Hotel> Hoteli { get; set; } = new();
    }
}