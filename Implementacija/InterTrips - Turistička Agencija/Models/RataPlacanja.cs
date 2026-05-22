using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class RataPlacanja
    {
        [Key]
        public int Id { get; set; }
        public int PlacanjeId { get; set; }
        public Placanje? Placanje { get; set; }
        public decimal IznosRate { get; set; }
        public DateTime RokZaUplatu { get; set; }
        public DateTime? DatumUplate { get; set; }
        public bool IsUplaceno { get; set; } = false;
    }
}