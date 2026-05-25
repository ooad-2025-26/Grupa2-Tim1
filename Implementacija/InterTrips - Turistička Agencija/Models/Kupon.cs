using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Kupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Kod { get; set; } = string.Empty; 
        public int PopustProcenat { get; set; }
        public DateTime VaziDo { get; set; }
        public bool Iskoristen { get; set; } = false;
    }
}