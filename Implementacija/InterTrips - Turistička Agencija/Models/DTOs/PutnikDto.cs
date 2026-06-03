using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models.DTOs 
{
    public class PutnikDto
    {
        [Required(ErrorMessage = "Ime putnika je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 50 karaktera.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime putnika je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 50 karaktera.")]
        public string Prezime { get; set; } = string.Empty;

     
        public string Pol { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj pasoša je obavezan.")]
        [RegularExpression(@"^[A-Z0-9]{6,15}$", ErrorMessage = "Neispravan format pasoša (samo velika slova i brojevi, 6-15 karaktera).")]
        [StringLength(20, ErrorMessage = "Broj pasoša ne može biti duži od 20 karaktera.")]
        public string BrojPasosa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Državljanstvo putnika je obavezno.")]
        [StringLength(100, ErrorMessage = "Naziv države ne može biti duži od 100 karaktera.")]
        public string Drzavljanstvo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum rođenja putnika je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime? DatumRodjenja { get; set; }

        [StringLength(20, ErrorMessage = "Telefon ne može biti duži od 20 karaktera.")]
        public string? Telefon { get; set; }

        [StringLength(1000, ErrorMessage = "Posebni zahtjevi ne mogu biti duži od 1000 karaktera.")]
        public string? PosebniZahtjevi { get; set; }
    }
}