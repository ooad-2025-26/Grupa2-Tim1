using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models;

public class StavkaPlanaTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PlanPutovanjaTemplateId { get; set; }
    public PlanPutovanjaTemplate? PlanPutovanjaTemplate { get; set; }

    public int RedniBroj { get; set; }

    [Required, MaxLength(160)]
    public string Naziv { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(max)")]
    public string? Opis { get; set; }

    public TimeSpan? Vrijeme { get; set; }
}