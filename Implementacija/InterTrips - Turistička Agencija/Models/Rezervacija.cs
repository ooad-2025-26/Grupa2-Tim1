using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class Rezervacija
{
    public Rezervacija() { }

    [Key]
    public int Id { get; set; }

    [Required]
    public DateOnly DatumPolaska { get; set; }

    [Required]
    public DateOnly DatumPovratka { get; set; }

    [Required]
    public StatusRezervacije Status { get; set; } = StatusRezervacije.Kreirana;

    [Required]
    public int PaketId { get; set; }
    public Paket? Paket { get; set; }

    [Required]
    public int KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }

    public List<Putnik> Putnici { get; set; } = new();

    public Placanje? Placanje { get; set; }

    public PlanPutovanja? PlanPutovanja { get; set; }

}