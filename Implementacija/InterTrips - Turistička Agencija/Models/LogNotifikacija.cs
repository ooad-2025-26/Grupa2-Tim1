using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class LogNotifikacija
    {
        [Key]
        public int Id { get; set; }

        public int? RezervacijaId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150, ErrorMessage = "Email primaoca ne može biti duži od 150 karaktera.")]
        public string EmailPrimaoca { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Tip notifikacije ne može biti duži od 50 karaktera.")]
        public string TipNotifikacije { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Status ne može biti duži od 50 karaktera.")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Range(0, 10, ErrorMessage = "Broj pokušaja mora biti između 0 i 10.")]
        public int BrojPokusaja { get; set; } = 1;

        [Required]
        public DateTime VrijemeSlanja { get; set; } = DateTime.Now;

        [MaxLength(4000, ErrorMessage = "Poruka greške ne može biti duža od 4000 karaktera.")]
        public string? PorukaGreske { get; set; } = string.Empty;

        public bool Procitana { get; set; } = false;

        public DateTime? DatumProcitano { get; set; }

        [ForeignKey(nameof(RezervacijaId))]
        public virtual Rezervacija? Rezervacija { get; set; }
    }
}