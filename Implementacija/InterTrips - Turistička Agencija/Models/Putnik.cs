using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class Putnik
{
    public Putnik() { }

    [Key]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Ime { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Prezime { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? BrojPasosa { get; set; }

    [MaxLength(80)]
    public string? Drzavljanstvo { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Telefon { get; set; }

    public DateTime? DatumRodjenja { get; set; }

    [MaxLength(600)]
    public string? PosebniZahtjevi { get; set; }

    [Required]
    public int RezervacijaId { get; set; }
    public Rezervacija? Rezervacija { get; set; }

    public int? KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }

}