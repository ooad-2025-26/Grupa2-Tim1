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
        public int BrojZvjezdica { get; set; }
        public int UkupnoSoba { get; set; }
        public int DostupnoSoba { get; set; }
        public decimal? CijenaPoNoci { get; set; }

        public string DostupneUsluge { get; set; } = string.Empty; 
        public string KontaktInformacije { get; set; } = string.Empty;
        public int DestinacijaId { get; set; }
        public Destinacija Destinacija { get; set; }
    }
}