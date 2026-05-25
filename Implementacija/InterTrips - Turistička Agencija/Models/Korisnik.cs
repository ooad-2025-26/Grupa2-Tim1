using InterTrips___Turistička_Agencija.Enums;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class Korisnik
{

    public Korisnik() { }

    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Ime { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Lozinka { get; set; } = string.Empty;


    [Required]
    public Uloga Uloga { get; set; } = Uloga.Klijent;

}