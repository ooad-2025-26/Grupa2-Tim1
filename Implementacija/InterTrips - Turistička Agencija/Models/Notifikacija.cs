using InterTrips___Turistička_Agencija.Enums;
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class Notifikacija
{

    public Notifikacija() { }

    [Key]
    public int Id { get; set; }

    [Required]
    public TipNotifikacije Tip { get; set; }

    [Required, MaxLength(300)]
    public string Poruka { get; set; } = string.Empty;

    public DateTime Vrijeme { get; set; } = DateTime.UtcNow;

    public bool Poslano { get; set; } = false;

    [Required]
    public int KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }
}