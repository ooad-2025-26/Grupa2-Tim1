using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class StavkaPlanaTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Stavka mora pripadati šablonu plana putovanja.")]
        public int PlanPutovanjaTemplateId { get; set; }

        [ForeignKey(nameof(PlanPutovanjaTemplateId))]
        public virtual PlanPutovanjaTemplate? PlanPutovanjaTemplate { get; set; }

        [Required(ErrorMessage = "Redni broj stavke je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Redni broj mora biti pozitivan broj (1 ili veći).")]
        public int RedniBroj { get; set; }

        [Required(ErrorMessage = "Naziv aktivnosti ili dana je obavezan.")]
        [MaxLength(160, ErrorMessage = "Naziv ne može biti duži od 160 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Opis aktivnosti u šablonu ne može biti duži od 1000 karaktera.")]
        public string? Opis { get; set; }

        public TimeSpan? Vrijeme { get; set; }
    }
}