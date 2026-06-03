using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Ime je obavezno polje.")]
        [MaxLength(100, ErrorMessage = "Ime ne može biti duže od 100 karaktera.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno polje.")]
        [MaxLength(100, ErrorMessage = "Prezime ne može biti duže od 100 karaktera.")]
        public string Prezime { get; set; } = string.Empty;

        public int Uloga { get; set; }

    }
}