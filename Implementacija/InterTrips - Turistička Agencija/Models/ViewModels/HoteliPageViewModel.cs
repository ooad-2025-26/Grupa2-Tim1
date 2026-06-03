using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Models.ViewModels
{
    public class HoteliPageViewModel
    {
        public List<Hotel> Hoteli { get; set; } = new List<Hotel>();
        public Hotel NoviHotel { get; set; } = new Hotel();
    }
}
