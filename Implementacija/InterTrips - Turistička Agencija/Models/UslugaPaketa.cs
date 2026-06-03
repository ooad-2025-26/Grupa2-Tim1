using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class UslugaPaketa
    {
        public UslugaPaketa() { }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        [MaxLength(120, ErrorMessage = "Naziv usluge ne može biti duži od 120 karaktera.")]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Opis usluge ne može biti duži od 500 karaktera.")] // Blago povećano radi detaljnijeg objašnjenja šta usluga obuhvata
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Usluga mora biti vezana za turistički paket.")]
        public int PaketId { get; set; }

        [ForeignKey(nameof(PaketId))]
        public virtual Paket? Paket { get; set; }
    }
}