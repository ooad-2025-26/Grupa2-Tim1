using System;

namespace InterTrips___Turistička_Agencija.Models
{
    public class LogNotifikacija
    {
        public int Id { get; set; }
        public int? RezervacijaId { get; set; }
        public string EmailPrimaoca { get; set; } = string.Empty;
        public string TipNotifikacije { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty; 
        public int BrojPokusaja { get; set; } = 1;
        public DateTime VrijemeSlanja { get; set; } = DateTime.Now;
        public string PorukaGreske { get; set; } = string.Empty;
        public bool Procitana { get; set; } = false;
        public DateTime? DatumProcitano { get; set; }

    }
}