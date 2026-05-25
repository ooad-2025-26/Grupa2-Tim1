# ✈️ InterTrips - Turistička Agencija

**Sistem za upravljanje turističkom agencijom koji omogućava rezervaciju putovanja, upravljanje paketima i finansijama te generisanje planova putovanja.**

![InterTrips Logo](https://github.com/user-attachments/assets/d2d97bfc-5d9e-4676-845d-1460f6837ef3)

---

## O projektu

**InterTrips** je web aplikacija namijenjena upravljanju poslovnim procesima turističke agencije. Sistem omogućava pregled i rezervaciju turističkih paketa, upravljanje terminima putovanja, unos podataka putnika, online plaćanje, administraciju sadržaja i pregled vlastitih rezervacija kroz korisničke panele.

Aplikacija je razvijena kao projektni zadatak i obuhvata funkcionalnosti za **klijente**, **turističke agente** i **administratore**.

---

## Tehnologije

- **Backend:** .NET 10 / ASP.NET Core MVC
- **Baza podataka:** Microsoft SQL Server
- **ORM:** Entity Framework Core 10
- **Autentifikacija:** ASP.NET Core Identity
- **Frontend:** Razor Views, HTML5, CSS3, JavaScript
- **Generisanje PDF dokumenata:** integrisani PDF engine na backendu
- **Pozadinski servisi:** `BackgroundWorker` za automatske podsjetnike putem e-maila

---

## Glavne funkcionalnosti

- pregled turističkih paketa i destinacija
- pregled dostupnih termina putovanja
- rezervacija putovanja
- unos podataka putnika
- online plaćanje rezervacije
- plaćanje na rate
- prikaz plana putovanja
- pregled vlastitih rezervacija
- administracija paketa, destinacija, hotela, letova i korisnika
- automatsko slanje e-mail podsjetnika 3 dana prije polaska
- generisanje PDF planova putovanja i prateće dokumentacije

---

## Uloge korisnika

Sistem podržava sljedeće korisničke uloge:

- **Administrator** – puni pristup admin panelu i upravljanje sistemom
- **Turistički agent** – upravljanje ponudom, paketima i rezervacijama
- **Klijent / turist** – pregled ponude, rezervacija i plaćanja

---

## Testni podaci za prijavu

Aplikacija koristi seedovane korisničke naloge za testiranje.

| Uloga | E-mail adresa | Lozinka | Opis |
| --- | --- | --- | --- |
| Administrator | `admin@intertrips.ba` | `admin123` | Pristup admin panelu i upravljanje sistemom |
| Turistički agent | `agent@intertrips.ba` | `agent123` | Upravljanje turističkim paketima i ponudom |
| Klijent / turist | `test@intertrips.ba` | `password123` | Kreiranje i pregled rezervacija |

---

##  Konekcijski string

Za povezivanje sa bazom podataka koristi se sljedeći connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SQL1004.site4now.net;Initial Catalog=db_ac97b2_intertrips;User Id=db_ac97b2_intertrips_admin;Password=" ";Encrypt=True;TrustServerCertificate=True;"
  }
}
