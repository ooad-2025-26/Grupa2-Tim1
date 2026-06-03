using InterTrips___Turistička_Agencija.Enums;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class Korisnik
{

    public Korisnik() { }

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ime je obavezno.")]
    [MaxLength(120, ErrorMessage = "Ime ne može biti duže od 120 karaktera.")]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [MaxLength(120, ErrorMessage = "Prezime ne može biti duže od 120 karaktera.")]
    public string Prezime { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Lozinka { get; set; } = string.Empty;


    [Required]
    public Uloga Uloga { get; set; } = Uloga.Klijent;

}