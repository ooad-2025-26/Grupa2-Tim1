using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Models
{
    public class PutnikDto
    {
        public string Ime { get; set; }

        public string Prezime { get; set; }

        public string BrojPasosa { get; set; }

        public string Drzavljanstvo { get; set; }

        public DateTime DatumRodjenja { get; set; }
        public string? Telefon { get; set; }
        public string? PosebniZahtjevi { get; set; }
    }
}