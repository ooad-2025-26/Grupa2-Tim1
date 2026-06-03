using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class TerminPutovanja : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Termin mora biti vezan za turistički paket.")]
        public int PaketId { get; set; }

        [ForeignKey(nameof(PaketId))]
        public virtual Paket? Paket { get; set; }

        [Required(ErrorMessage = "Datum polaska je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumPolaska { get; set; }

        [Required(ErrorMessage = "Datum povratka je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumPovratka { get; set; }

        [Required(ErrorMessage = "Kapacitet termina je obavezan.")]
        [Range(1, 1000, ErrorMessage = "Kapacitet mora biti između 1 i 1000 mjesta.")]
        public int Kapacitet { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Broj popunjenih mjesta ne može biti negativan.")]
        public int Popunjeno { get; set; } = 0;

        [NotMapped]
        public int Slobodno => Kapacitet - Popunjeno;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DatumPolaska.Date < DateTime.Today)
            {
                yield return new ValidationResult("Datum polaska ne može biti u prošlosti.", new[] { nameof(DatumPolaska) });
            }

            if (DatumPovratka <= DatumPolaska)
            {
                yield return new ValidationResult("Datum povratka mora biti nakon datuma polaska.", new[] { nameof(DatumPovratka) });
            }

            if (Popunjeno > Kapacitet)
            {
                yield return new ValidationResult("Broj popunjenih mjesta ne može biti veći od ukupnog kapaciteta.", new[] { nameof(Popunjeno) });
            }
        }
    }
}