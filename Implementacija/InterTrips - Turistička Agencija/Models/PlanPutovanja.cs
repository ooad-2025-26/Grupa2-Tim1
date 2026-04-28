using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class PlanPutovanja
{

    public PlanPutovanja() { }

    [Key]
    public int Id { get; set; }

    [MaxLength(800)]
    public string? Napomena { get; set; }

    [Required]
    public int RezervacijaId { get; set; }
    public Rezervacija? Rezervacija { get; set; }

    public List<StavkaPlana> Stavke { get; set; } = new();
}

public class StavkaPlana
{
    public StavkaPlana() { }

    [Key]
    public int Id { get; set; }

    [Required]
    public int PlanPutovanjaId { get; set; }
    public PlanPutovanja? PlanPutovanja { get; set; }

    public int RedniBroj { get; set; }

    [Required, MaxLength(160)]
    public string Naziv { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Opis { get; set; }

    public DateTime? DatumVrijeme { get; set; }
}