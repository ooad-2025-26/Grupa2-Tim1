using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Nevaljan format email adrese.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Lozinka mora imati minimalno 8 karaktera.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo, jedno malo slovo i jedan broj.")]
        public string Password { get; set; } = string.Empty;
    }
}