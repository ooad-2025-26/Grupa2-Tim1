using InterTrips___Turistička_Agencija.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InterTrips___Turistička_Agencija.Models;

public class Rezervacija
{
    public Rezervacija() { }

    [Key]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DatumPolaska { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DatumPovratka { get; set; }

    [Required]
    public StatusRezervacije Status { get; set; } = StatusRezervacije.Kreirana;

    [Required]
    public int PaketId { get; set; }
    public Paket? Paket { get; set; }

    [Required]
    public string KorisnikId { get; set; }

    [ForeignKey("KorisnikId")]
   public ApplicationUser? Korisnik { get; set; }
    [Required]
    public string TipSobe { get; set; } = "Standardna soba";

    [Required]
    public VrstaPrevoza TipPrevoza { get; set; }

    public List<Putnik> Putnici { get; set; } = new();

    public Placanje? Placanje { get; set; }

    public PlanPutovanja? PlanPutovanja { get; set; }

}