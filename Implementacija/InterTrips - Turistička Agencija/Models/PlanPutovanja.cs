using System.Collections.Generic;
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

    public List<StavkaPlana> StavkePlana { get; set; } = new();
}

