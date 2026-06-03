using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InterTrips___Turistička_Agencija.Models.ViewModels 
{
    public class DodajDestinacijuVm
    {
        [Required(ErrorMessage = "Naziv destinacije je obavezan.")]
        [MaxLength(120, ErrorMessage = "Naziv ne može biti duži od 120 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naziv države je obavezan.")]
        [MaxLength(120, ErrorMessage = "Naziv države ne može biti duži od 120 karaktera.")]
        public string Drzava { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Kategorija ne može biti duža od 50 karaktera.")]
        public string? Kategorija { get; set; }

        [MaxLength(1000, ErrorMessage = "Opis ne može biti duži od 1000 karaktera.")]
        public string? Opis { get; set; }

       
    }
}