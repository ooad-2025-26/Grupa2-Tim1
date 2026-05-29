using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class KontaktUpit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ime { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Tema { get; set; }

        [Required]
        public string Poruka { get; set; }

        public DateTime DatumSlanja { get; set; } = DateTime.Now;

        public bool Procitano { get; set; } = false; 
    }
}