using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Paket
    {
        public Paket() { }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(160)]
        public string Naziv { get; set; } = string.Empty;

        [Required]
        public StatusPaketa Status { get; set; } = StatusPaketa.Dostupan;

        [Required, Range(0, 100000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CijenaOd { get; set; } 

        [Range(1, 365)]
        public int TrajanjeNoci { get; set; }

        [Required]
        public int DestinacijaId { get; set; }
        public virtual Destinacija? Destinacija { get; set; }

        [ValidateNever]
        public int Kapacitet { get; set; } = 30;

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
    }
}