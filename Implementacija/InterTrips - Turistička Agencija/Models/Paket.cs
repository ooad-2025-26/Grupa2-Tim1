using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models;

public class Paket
{

    public Paket() { }

    [Key]
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Naziv { get; set; } = string.Empty;

    [Required]
    public StatusPaketa Status { get; set; } = StatusPaketa.Dostupan;

    [Required, Range(0, 100000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CijenaOd { get; set; }

    [Range(1, 365)]
    public int TrajanjeNoci { get; set; }

    [Required]
    public int DestinacijaId { get; set; }
    public Destinacija? Destinacija { get; set; }

    public List<UslugaPaketa> Usluge { get; set; } = new();

    public List<Rezervacija> Rezervacije { get; set; } = new();

    [MaxLength(260)]
    public string? SlikaUrl { get; set; }

    [MaxLength(800)]
    public string? Opis { get; set; }
}