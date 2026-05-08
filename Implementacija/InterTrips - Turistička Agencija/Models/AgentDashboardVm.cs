using InterTrips___Turistička_Agencija.Models;

namespace InterTrips___Turistička_Agencija.Models.ViewModels;

public class AgentDashboardVm
{
    public int AgentId { get; set; }
    public List<Paket> SviPaketi { get; set; } = new();
    public HashSet<int> MojiPaketiIds { get; set; } = new();
    public List<Rezervacija> AktivneRezervacije { get; set; } = new();
}