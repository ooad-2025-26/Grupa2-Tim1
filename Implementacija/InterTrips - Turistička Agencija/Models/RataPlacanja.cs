using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class RataPlacanja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlacanjeId { get; set; }

        [ForeignKey(nameof(PlacanjeId))]
        public virtual Placanje? Placanje { get; set; }

        [Required(ErrorMessage = "Iznos rate je obavezan.")]
        [Range(0.01, 100000.00, ErrorMessage = "Iznos rate mora biti veći od 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal IznosRate { get; set; }

        [Required(ErrorMessage = "Rok za uplatu rate je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime RokZaUplatu { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DatumUplate { get; set; }

        public bool IsUplaceno { get; set; } = false;
    }
}