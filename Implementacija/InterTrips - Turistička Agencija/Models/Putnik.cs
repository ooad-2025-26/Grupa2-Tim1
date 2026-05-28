using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Putnik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Ime { get; set; }

        [Required]
        [StringLength(50)]
        public string Prezime { get; set; }

        [Required]
        [StringLength(1)]
        public string Pol { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatumRodjenja { get; set; } 

        [Required]
        [StringLength(20)]
        public string Telefon { get; set; }

        [Required]
        [StringLength(20)]
        public string BrojPasosa { get; set; }

        [Required]
        [StringLength(100)]
        public string Drzavljanstvo { get; set; }

        public string PosebniZahtjevi { get; set; }

        public int? RezervacijaId { get; set; }
        public string TipPutnika { get; set; } = string.Empty;
    }
}