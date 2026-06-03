using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv hotela je obavezan.")]
        [MaxLength(150, ErrorMessage = "Naziv ne može biti duži od 150 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lokacija/Adresa hotela je obavezna.")]
        [MaxLength(200, ErrorMessage = "Lokacija ne može biti duža od 200 karaktera.")]
        public string Lokacija { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Broj zvjezdica mora biti između 0 i 5.")]
        public int BrojZvjezdica { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Broj slobodnih soba ne može biti negativan.")]
        public int DostupnoSoba { get; set; }

        [MaxLength(1000, ErrorMessage = "Opis dostupnih usluga ne može biti duži od 1000 karaktera.")]
        public string DostupneUsluge { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Kontakt informacije ne mogu biti duže od 250 karaktera.")]
        public string KontaktInformacije { get; set; } = string.Empty;

        [Required]
        public int DestinacijaId { get; set; }

        [ForeignKey(nameof(DestinacijaId))]
        public virtual Destinacija? Destinacija { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Paket> Paketi { get; set; } = new List<Paket>();

        public bool IsActive { get; set; } = true;
    }
}