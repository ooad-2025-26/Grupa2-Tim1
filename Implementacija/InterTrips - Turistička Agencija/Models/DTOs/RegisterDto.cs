using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models.DTOs 
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 50 karaktera.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 50 karaktera.")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Nevaljan format email adrese.")]
        [StringLength(150, ErrorMessage = "Email ne može biti duži od 150 karaktera.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Lozinka mora imati minimalno 8 karaktera.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo, jedno malo slovo i jedan broj.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [DataType(DataType.Password)]
        [Compare("Lozinka", ErrorMessage = "Lozinke se ne podudaraju.")]
        public string PotvrdiLozinku { get; set; } = string.Empty;
    }
}