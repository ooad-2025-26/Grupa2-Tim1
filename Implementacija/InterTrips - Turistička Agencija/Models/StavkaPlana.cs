using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;


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