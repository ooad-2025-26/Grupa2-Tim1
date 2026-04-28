using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class UslugaPaketa
{
    public UslugaPaketa() { }

    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Naziv { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Opis { get; set; }

    [Required]
    public int PaketId { get; set; }
    public Paket? Paket { get; set; }

}