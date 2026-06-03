using System.Collections.Generic;
using InterTrips___Turistička_Agencija.Models; 
namespace InterTrips___Turistička_Agencija.Models.ViewModels
{
    public class AdminRezervacijeVm
    {
        public string? KorisnikEmail { get; set; }

        public string? StatusFilter { get; set; }

        public List<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
    }
}