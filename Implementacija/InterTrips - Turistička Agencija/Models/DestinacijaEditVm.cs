using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace InterTrips___Turistička_Agencija.Models;

public class DestinacijaEditVm
{
    public int? Id { get; set; }

    [Required] public string Naziv { get; set; } = "";
    [Required] public string Drzava { get; set; } = "";
    public string? Opis { get; set; }

    public IFormFile? Slika { get; set; }  
    public string? PostojecaSlikaUrl { get; set; }
}