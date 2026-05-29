using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Paket : IValidatableObject
    {
        public Paket() { }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(160)]
        public string Naziv { get; set; } = string.Empty;

        [Required]
        public StatusPaketa Status { get; set; } = StatusPaketa.Dostupan;

        [Required, Range(1.00, 100000.00, ErrorMessage = "Cijena mora biti veća od 1 BAM.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CijenaOd { get; set; }

        [Range(1, 365)]
        public int TrajanjeNoci { get; set; }

        [Required]
        public int DestinacijaId { get; set; }
        public virtual Destinacija? Destinacija { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Kapacitet ne može biti negativan.")]
        public int Kapacitet { get; set; }

        public DateTime DatumPolaska { get; set; }
        public DateTime DatumPovratka { get; set; }

        [ValidateNever]
        public List<UslugaPaketa> Usluge { get; set; } = new();
        
        [ValidateNever]
        public List<Rezervacija> Rezervacije { get; set; } = new();

        [MaxLength(260)]
        public string? SlikaUrl { get; set; }

        [MaxLength(800)]
        public string? Opis { get; set; }

        public int? HotelId { get; set; }
        [ForeignKey("HotelId")]
        public virtual Hotel? Hotel { get; set; }

        public int? LetId { get; set; }
        [ForeignKey("LetId")] 
        public virtual Let? Let { get; set; }
        public VrstaPrevoza DostupniPrevoz { get; set; } = VrstaPrevoza.Oboje;
        public List<TerminPutovanja> DostupniTermini { get; set; } = new List<TerminPutovanja>();


        public int BrojPregleda { get; set; } = 0;
        public double Ocjena { get; set; } = 0.0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateTime.UtcNow.Date > DatumPolaska.Date)
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