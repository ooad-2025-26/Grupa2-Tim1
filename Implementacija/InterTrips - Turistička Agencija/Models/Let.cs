using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Let
    {
        [Key]
        public int Id { get; set; }
        public string? Aviokompanija { get; set; } = string.Empty;
        [Required]
        public string BrojLeta { get; set; } = string.Empty;
        public DateTime VrijemePolaska { get; set; }
        public DateTime VrijemeDolaska { get; set; }
        public string TipAviona { get; set; } = string.Empty;
        public int UkupnoSjedista { get; set; }
        public int SlobodnaSjedista { get; set; }
        public string Polazak { get; set; }
        public string Odrediste { get; set; }
    }
}