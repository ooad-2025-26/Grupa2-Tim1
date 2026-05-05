

using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class DodajDestinacijuVm
    {
        [Required] public string Naziv { get; set; } = "";
        [Required] public string Drzava { get; set; } = "";
        public string? Kategorija { get; set; }
        public string? Opis { get; set; }
    }
}