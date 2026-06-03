using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class PlanPutovanja
    {
        public PlanPutovanja() { }

        [Key]
        public int Id { get; set; }

        [MaxLength(1000, ErrorMessage = "Napomena ne može biti duža od 1000 karaktera.")] 
        public string? Napomena { get; set; }

        [Required(ErrorMessage = "Plan putovanja mora biti vezan za rezervaciju.")]
        public int RezervacijaId { get; set; }

        [ForeignKey(nameof(RezervacijaId))]
        public virtual Rezervacija? Rezervacija { get; set; }

        public virtual List<StavkaPlana> StavkePlana { get; set; } = new List<StavkaPlana>();
    }
}