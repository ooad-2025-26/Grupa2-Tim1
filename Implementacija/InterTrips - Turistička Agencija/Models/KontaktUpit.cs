using System;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class KontaktUpit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime i prezime su obavezni.")]
        [MaxLength(150, ErrorMessage = "Ime ne može biti duže od 150 karaktera.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        [MaxLength(150, ErrorMessage = "Email ne može biti duži od 150 karaktera.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naslov ili tema poruke je obavezna.")]
        [MaxLength(200, ErrorMessage = "Tema ne može biti duža od 200 karaktera.")]
        public string Tema { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tekst poruke je obavezan.")]
        [MaxLength(4000, ErrorMessage = "Poruka ne može biti duža od 4000 karaktera.")]
        public string Poruka { get; set; } = string.Empty;

        public DateTime DatumSlanja { get; set; } = DateTime.Now;

        public bool Procitano { get; set; } = false;
    }
}