using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterTrips___Turistička_Agencija.Models
{
    public class TerminPutovanja
    {
        public int Id { get; set; }
        public int PaketId { get; set; }
        [ForeignKey("PaketId")]
        public virtual Paket Paket { get; set; }
        public DateTime DatumPolaska { get; set; }
        public DateTime DatumPovratka { get; set; }
        public int Kapacitet { get; set; }
        public int Popunjeno { get; set; }
    }
}
