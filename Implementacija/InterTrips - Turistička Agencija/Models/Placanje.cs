using InterTrips___Turistička_Agencija.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models;

public class Placanje
{

    public Placanje() { }

    [Key]
    public int Id { get; set; }

    [Required]
    public MetodaPlacanja Metoda { get; set; }

    [Required, Range(0, 100000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Iznos { get; set; }

    public DateTime VrijemePlacanja { get; set; } = DateTime.UtcNow;

    [Required]
    public int RezervacijaId { get; set; }
    public Rezervacija? Rezervacija { get; set; }
    public virtual ICollection<RataPlacanja> Rate { get; set; } = new List<RataPlacanja>();

}