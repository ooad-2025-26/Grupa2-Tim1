using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Dobavljac
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Naziv { get; set; } = string.Empty;
        public string VrstaUsluge { get; set; } = string.Empty; 
        public string KontaktOsoba { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public bool Aktivan { get; set; } = true;
    }
}