using InterTrips___Turistička_Agencija.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Rezervacija : IValidatableObject
    {
        public Rezervacija() { }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Datum polaska je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumPolaska { get; set; }

        [Required(ErrorMessage = "Datum povratka je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumPovratka { get; set; }

        [Required]
        public StatusRezervacije Status { get; set; } = StatusRezervacije.Kreirana;

        [Required(ErrorMessage = "Morate odabrati turistički paket.")]
        public int PaketId { get; set; }

        [ForeignKey(nameof(PaketId))]
        public virtual Paket? Paket { get; set; }

        [Required]
        public string KorisnikId { get; set; } = string.Empty;

        [ForeignKey(nameof(KorisnikId))]
        public virtual ApplicationUser? Korisnik { get; set; }

        [Required(ErrorMessage = "Tip sobe je obavezan.")]
        [MaxLength(100, ErrorMessage = "Naziv tipa sobe ne može biti duži od 100 karaktera.")]
        public string TipSobe { get; set; } = "Standardna soba";

        [Required(ErrorMessage = "Morate odabrati vrstu prevoza.")]
        public VrstaPrevoza TipPrevoza { get; set; }

        public virtual List<Putnik> Putnici { get; set; } = new List<Putnik>();

        public virtual Placanje? Placanje { get; set; }

        public virtual PlanPutovanja? PlanPutovanja { get; set; }

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
        }
    }
}