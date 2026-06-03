using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Models.ViewModels 
{
    public class AgentPaketiVm
    {
        public int AgentId { get; set; }

        public List<Paket> SviPaketi { get; set; } = new List<Paket>();

        public HashSet<int> MojiPaketiIds { get; set; } = new HashSet<int>();
    }
}