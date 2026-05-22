using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class LogNotifikacija
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PrimalacEmail { get; set; } = string.Empty;
        public string Naslov { get; set; } = string.Empty;
        public DateTime DatumSlanja { get; set; }
        public string StatusSlanja { get; set; } = "Uspješno"; // Uspješno, Greška, Spam folder
        public string DetaljiGreske { get; set; } = string.Empty;
        public int PokusajBroj { get; set; } = 1;
    }
}