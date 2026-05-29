using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Naziv { get; set; } = string.Empty;
        [Required]
        public string Lokacija { get; set; } = string.Empty;
        [Range(0, 5, ErrorMessage = "Broj zvjezdica je od 0 do 5.")]
        public int BrojZvjezdica { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Broj slobodnih soba ne može biti negativan.")]
        public int DostupnoSoba { get; set; }
        public string DostupneUsluge { get; set; } = string.Empty; 
        public string KontaktInformacije { get; set; } = string.Empty;
        public int DestinacijaId { get; set; }
        public virtual Destinacija? Destinacija { get; set; }

        [System.Text.Json.Serialization.JsonIgnore] 
        public virtual ICollection<Paket> Paketi { get; set; } = new List<Paket>();

        public bool IsActive { get; set; } = true;
    }
}