using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace InterTrips___Turistička_Agencija.Models
{
    public class AgentPaketiVm
    {
        public int AgentId { get; set; }

        public List<Paket> SviPaketi { get; set; } = new List<Paket>();

        public List<int> MojiPaketiIds { get; set; } = new List<int>();
    }
}
