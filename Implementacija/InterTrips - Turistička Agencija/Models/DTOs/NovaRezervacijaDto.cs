using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models.DTOs 
{
    public class NovaRezervacijaDto
    {
        [Required(ErrorMessage = "Morate odabrati paket aranžman.")]
        public int PaketId { get; set; }

        [Required(ErrorMessage = "Datum polaska je obavezan.")]
        public DateOnly DatumPolaska { get; set; }

        [Required(ErrorMessage = "Datum povratka je obavezan.")]
        public DateOnly DatumPovratka { get; set; }

        public List<PutnikDto> Putnici { get; set; } = new List<PutnikDto>();

        [MaxLength(50)]
        public string? PromoKod { get; set; }

        [MaxLength(20)]
        public string? BrojRezervacije { get; set; }

        [Required(ErrorMessage = "Način plaćanja je obavezan.")]
        [MaxLength(50)] 
        public string NacinPlacanja { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ukupan iznos je obavezan.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ukupan iznos mora biti veći od 0.")]
        public decimal UkupanIznos { get; set; }

        [Range(2, 24, ErrorMessage = "Broj mjeseci za rate mora biti između 2 i 24.")]
        public int? BrojMjeseciRata { get; set; }

        [MaxLength(50)] 
        public string? SubMetodaRata { get; set; }

        [Required]
        [MaxLength(100)]
        public string TipSobe { get; set; } = "Standardna soba";

        [Required]
        [MaxLength(100)]
        public string TipPrevoza { get; set; } = "Autobus";
    }
}