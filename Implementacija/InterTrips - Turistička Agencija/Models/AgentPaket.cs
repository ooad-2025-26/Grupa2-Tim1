using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class AgentPaket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        public int PaketId { get; set; }

        [ForeignKey(nameof(AgentId))]
        public Korisnik? Agent { get; set; }

        [ForeignKey(nameof(PaketId))]
        public Paket? Paket { get; set; }
    }
}