using System;

namespace InterTrips___Turistička_Agencija.Models
{
    public class NovaRezervacijaDto
    {
        public int PaketId { get; set; }
        public DateOnly DatumPolaska { get; set; }
        public DateOnly DatumPovratka { get; set; }

        public List<PutnikDto> Putnici { get; set; } = new();
        public string? PromoKod { get; set; }
        public string? BrojRezervacije { get; set; }
        public string NacinPlacanja { get; set; }
        public decimal UkupanIznos { get; set; }
        public int? BrojMjeseciRata { get; set; }
        public string? SubMetodaRata { get; set; }
    }
}