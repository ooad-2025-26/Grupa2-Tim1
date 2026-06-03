using InterTrips___Turistička_Agencija.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Placanje
    {
        public Placanje() { }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Metoda plaćanja je obavezna.")]
        public MetodaPlacanja Metoda { get; set; }

        [Required(ErrorMessage = "Iznos uplate je obavezan.")]
        [Range(0.00, 100000.00, ErrorMessage = "Iznos ne može biti negativan.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Iznos { get; set; }

        public DateTime VrijemePlacanja { get; set; } = DateTime.Now;

        [Required]
        public int RezervacijaId { get; set; }

        [ForeignKey(nameof(RezervacijaId))]
        public virtual Rezervacija? Rezervacija { get; set; }

        public virtual ICollection<RataPlacanja> Rate { get; set; } = new List<RataPlacanja>();

        public int? KuponId { get; set; }

        [ForeignKey(nameof(KuponId))]
        public virtual Kupon? Kupon { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.00, 100000.00, ErrorMessage = "Originalni iznos ne može biti negativan.")]
        public decimal? OriginalniIznos { get; set; }

        [MaxLength(150, ErrorMessage = "Transakcijski kod ne može biti duži od 150 karaktera.")]
        public string TransakcijskiKod { get; set; } = string.Empty;
    }
}