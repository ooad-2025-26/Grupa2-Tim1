using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InterTrips___Turistička_Agencija.Enums;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Notifikacija
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tip notifikacije je obavezan.")]
        public TipNotifikacije Tip { get; set; }

        [Required(ErrorMessage = "Tekst poruke je obavezan.")]
        [MaxLength(300, ErrorMessage = "Poruka ne može biti duža od 300 karaktera.")]
        public string Poruka { get; set; } = string.Empty;

        public DateTime Vrijeme { get; set; } = DateTime.Now;

        public bool Poslano { get; set; } = false;

        [Required]
        public string KorisnikId { get; set; } = string.Empty;
        
        [ForeignKey(nameof(KorisnikId))]
        public virtual ApplicationUser? Korisnik { get; set; }
    }
}