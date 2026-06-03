using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class StavkaPlana
    {
        public StavkaPlana() { }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Stavka mora pripadati planu putovanja.")]
        public int PlanPutovanjaId { get; set; }

        [ForeignKey(nameof(PlanPutovanjaId))]
        public virtual PlanPutovanja? PlanPutovanja { get; set; }

        [Required(ErrorMessage = "Redni broj stavke je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Redni broj mora biti pozitivan broj (veći od 0).")]
        public int RedniBroj { get; set; }

        [Required(ErrorMessage = "Naziv aktivnosti ili dana je obavezan.")]
        [MaxLength(160, ErrorMessage = "Naziv ne može biti duži od 160 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(800, ErrorMessage = "Opis aktivnosti ne može biti duži od 800 karaktera.")] // Blago povećano radi detaljnijeg opisa izleta
        public string? Opis { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DatumVrijeme { get; set; }
    }
}