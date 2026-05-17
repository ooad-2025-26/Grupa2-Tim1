using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Models.ViewModels
{
    public class AdminRezervacijeVm
    {
        public string KorisnikEmail { get; set; } = "test@intertrips.ba";
        public List<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
    }
}