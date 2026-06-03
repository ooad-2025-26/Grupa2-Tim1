using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Kupon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kod kupona je obavezan.")]
        [MaxLength(50, ErrorMessage = "Kod kupona ne može biti duži od 50 karaktera.")]
        public string Kod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Procenat popusta je obavezan.")]
        [Range(1, 100, ErrorMessage = "Popust mora biti između 1% i 100%.")]
        public int PopustProcenat { get; set; }

        [Required(ErrorMessage = "Datum isteka kupona je obavezan.")]
        public DateTime VaziDo { get; set; }

        public bool Iskoristen { get; set; } = false;

       public bool IsValid => !Iskoristen && VaziDo >= DateTime.Now;
    }
}