using System.ComponentModel.DataAnnotations;

namespace InterTrips___Turistička_Agencija.Models;

public class AgentPaket
{
    [Key]
    public int Id { get; set; }

    public int AgentId { get; set; }
    public int PaketId { get; set; }

    public Korisnik? Agent { get; set; }
    public Paket? Paket { get; set; }
}