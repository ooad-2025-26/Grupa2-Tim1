using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Models
{
    public class PutnikDto
    {
        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public string BrojPasosa { get; set; } = string.Empty;

        public string Drzavljanstvo { get; set; } = string.Empty;

        public DateTime DatumRodjenja { get; set; }
        public string? Telefon { get; set; }
        public string? PosebniZahtjevi { get; set; }
    }
}