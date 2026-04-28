
namespace InterTrips___Turistička_Agencija.Models;

public enum Uloga
{
    Klijent = 0,
    Agent = 1,
    Admin = 2
}

public enum StatusPaketa
{
    Dostupan = 0,
    Rasprodan = 1
}

public enum StatusRezervacije
{
    Kreirana = 0,
    Potvrdjena = 1,
    Otkazana = 2
}

public enum MetodaPlacanja
{
    Kartica = 0,
    BankovniTransfer = 1,
    Gotovina = 2,
    Rate = 3
}

public enum TipNotifikacije
{
    PotvrdaRezervacije = 0,
    PotvrdaPlacanja = 1
}