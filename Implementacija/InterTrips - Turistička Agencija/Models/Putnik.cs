using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Putnik
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime putnika je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 50 karaktera.")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime putnika je obavezno.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 50 karaktera.")]
        public string Prezime { get; set; }

        [Required]
        [StringLength(1)]
        public string Pol { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatumRodjenja { get; set; } 

        [Required]
        [StringLength(20)]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Broj pasoša je obavezan.")]
        [RegularExpression(@"^[A-Z0-9]{6,15}$", ErrorMessage = "Neispravan format pasoša (samo velika slova i brojevi, 6-15 karaktera).")]
        [StringLength(20)]
        public string BrojPasosa { get; set; }

        [Required]
        [StringLength(100)]
        public string Drzavljanstvo { get; set; }

        public string PosebniZahtjevi { get; set; }

        public int? RezervacijaId { get; set; }
        public string TipPutnika { get; set; } = string.Empty;
    }
}