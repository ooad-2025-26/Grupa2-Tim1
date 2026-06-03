using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Paket : IValidatableObject
    {
        public Paket() { }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv paketa je obavezan.")]
        [MaxLength(160, ErrorMessage = "Naziv ne može biti duži od 160 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [Required]
        public StatusPaketa Status { get; set; } = StatusPaketa.Dostupan;

        [Required(ErrorMessage = "Početna cijena je obavezna.")]
        [Range(1.00, 100000.00, ErrorMessage = "Cijena mora biti između 1 i 100.000 BAM.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CijenaOd { get; set; }

        [Range(1, 365, ErrorMessage = "Trajanje mora biti između 1 i 365 noćenja.")]
        public int TrajanjeNoci { get; set; }

        [Required(ErrorMessage = "Morate odabrati destinaciju.")]
        public int DestinacijaId { get; set; }

        [ForeignKey(nameof(DestinacijaId))]
        public virtual Destinacija? Destinacija { get; set; }

        [Required(ErrorMessage = "Kapacitet paketa je obavezan.")]
        [Range(0, int.MaxValue, ErrorMessage = "Kapacitet ne može biti negativan.")]
        public int Kapacitet { get; set; }

        [Required(ErrorMessage = "Datum polaska je obavezan.")]
        public DateTime DatumPolaska { get; set; }

        [Required(ErrorMessage = "Datum povratka je obavezan.")]
        public DateTime DatumPovratka { get; set; }

        [ValidateNever]
        public List<UslugaPaketa> Usluge { get; set; } = new();

        [ValidateNever]
        public List<Rezervacija> Rezervacije { get; set; } = new();

        [MaxLength(500, ErrorMessage = "URL slike ne može biti duži od 500 karaktera.")] // Povećano radi sigurnosti dugih eksternih URL-ova
        public string? SlikaUrl { get; set; }

        [MaxLength(1000, ErrorMessage = "Opis ne može biti duži od 1000 karaktera.")] // Blago povećano za detaljniji opis programa putovanja
        public string? Opis { get; set; }

        public int? HotelId { get; set; }
        [ForeignKey(nameof(HotelId))]
        public virtual Hotel? Hotel { get; set; }

        public int? LetId { get; set; }
        [ForeignKey(nameof(LetId))]
        public virtual Let? Let { get; set; }

        public VrstaPrevoza DostupniPrevoz { get; set; } = VrstaPrevoza.Oboje;

        [ValidateNever]
        public List<TerminPutovanja> DostupniTermini { get; set; } = new List<TerminPutovanja>();

        public int BrojPregleda { get; set; } = 0;
        public double Ocjena { get; set; } = 0.0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateTime.Now.Date > DatumPolaska.Date)
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