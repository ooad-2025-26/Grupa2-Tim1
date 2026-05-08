
using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models
{
    public class Destinacija
{
    public Destinacija() { }

    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Naziv { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Drzava { get; set; } = string.Empty;

    [MaxLength(40)]
    public string? Kategorija { get; set; }

    [MaxLength(800)]
    public string? Opis { get; set; }

    [MaxLength(260)]
    public string? SlikaUrl { get; set; }
    public List<Paket> Paketi { get; set; } = new();
}
}
