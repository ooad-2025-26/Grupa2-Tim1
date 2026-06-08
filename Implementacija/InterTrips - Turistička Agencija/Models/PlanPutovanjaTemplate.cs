using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class PlanPutovanjaTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Šablon plana mora biti vezan za turistički paket.")]
        public int PaketId { get; set; }

        [ForeignKey(nameof(PaketId))]
        public virtual Paket? Paket { get; set; }

        [MaxLength(1000, ErrorMessage = "Napomena ne može biti duža od 1000 karaktera.")] 
        public string? Napomena { get; set; }

        public virtual List<StavkaPlanaTemplate> Stavke { get; set; } = new List<StavkaPlanaTemplate>();
    }
}