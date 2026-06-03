using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Let
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Aviokompanija je obavezno polje.")]
        [MaxLength(100, ErrorMessage = "Naziv aviokompanije ne može biti duži od 100 karaktera.")]
        public string Aviokompanija { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj leta je obavezan (npr. JU 100).")]
        [MaxLength(20, ErrorMessage = "Broj leta ne može biti duži od 20 karaktera.")]
        public string BrojLeta { get; set; } = string.Empty;


        public DateTime? VrijemePolaska { get; set; }

        public DateTime? VrijemeDolaska { get; set; }


        [Required(ErrorMessage = "Ukupan broj sjedišta je obavezan.")]
        [Range(1, 1000, ErrorMessage = "Ukupan broj sjedišta mora biti između 1 i 1000.")]
        public int UkupnoSjedista { get; set; }

        [Required(ErrorMessage = "Broj slobodnih sjedišta je obavezan.")]
        [Range(0, 1000, ErrorMessage = "Broj slobodnih sjedišta ne može biti negativan.")]
        public int SlobodnaSjedista { get; set; }

        [Required(ErrorMessage = "Mjesto polaska je obavezno.")]
        [MaxLength(100, ErrorMessage = "Naziv polazišta ne može biti duži od 100 karaktera.")]
        public string Polazak { get; set; } = string.Empty;

        [Required(ErrorMessage = "Odredište je obavezno.")]
        [MaxLength(100, ErrorMessage = "Naziv odredišta ne može biti duži od 100 karaktera.")]
        public string Odrediste { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore] 
        public virtual ICollection<Paket> Paketi { get; set; } = new List<Paket>();
    }
}