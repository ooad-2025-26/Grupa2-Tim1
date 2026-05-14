using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class PlanPutovanjaTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PaketId { get; set; }
    public Paket? Paket { get; set; }

    [MaxLength(800)]
    public string? Napomena { get; set; }

    public List<StavkaPlanaTemplate> Stavke { get; set; } = new();
}