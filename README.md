# ✈️ InterTrips - Turistička Agencija 

**Sistem za upravljanje turističkom agencijom koji omogućava rezervaciju putovanja, upravljanje paketima i finansijama te generisanje planova putovanja.**

![InterTrips Logo](https://github.com/user-attachments/assets/d2d97bfc-5d9e-4676-845d-1460f6837ef3)

---

## O Projektu

**InterTrips** je informatički sistem namijenjen upravljanju poslovnim procesima turističke agencije u obliku ASP.NET Core MVC web-aplikacije. Sistem omogućava klijentima pregledavanje raznovrsnih turističkih paketa, online rezervaciju putovanja, upravljanje plaćanjima na rate i preuzimanje službenih itinerera u PDF formatu. 

Agenti i administratori kroz napredni pozadinski panel mogu upravljati paketima, letovima, hotelskim smještajem, pratiti uplate, te komunicirati sa klijentima. Aplikacija posjeduje i pozadinske servise (`BackgroundWorker`) koji automatski provjeravaju bazu i šalju e-mail podsjetnike klijentima 3 dana prije polaska na putovanje.

---

## Tech Stack

* **Backend:** .NET 10 (ASP.NET Core MVC)
* **Baza podataka:** Microsoft SQL Server 
* **ORM:** Entity Framework Core (EF Core) 10
* **Autentifikacija:** ASP.NET Core Identity (Sistem uloga: Admin, Agent, Klijent)
* **Frontend:** Razor Views, HTML5, CSS3 (Elegantni custom UI), JavaScript (Fetch API)
* **Generisanje dokumenata:** PDF Engine integrisan na backendu 

---

## Testni podaci za prijavu

Aplikacija dolazi sa pre-definisanim (seedovanim) korisničkim računima unutar baze podataka kako bi se olakšalo testiranje autorizacije i funkcionalnosti specifičnih za uloge:

| Uloga | E-mail adresa | Lozinka | Opis i dozvole |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin@intertrips.ba` | `admin123` | Pristup kontrolnoj tabli `/Administrator`, upravljanje svim korisnicima, ulogama i finansijskim izvještajima, dodavanje hotela i letova. |
| **Turistički Agent** | `agent@intertrips.ba` | `agent123` | Pristup panelu `/Agent`, kreiranje i ažuriranje turističkih paketa |
| **Klijent / Turist** | `test@intertrips.ba` | `password123` | Profil klijenta, pregled vlastitih rezervacija, kreiranje rezervacija |

---

## Konekcijski String (Database Connection String)

Za pokretanje aplikacije i povezivanje sa centralnom bazom podataka,:

```json
{
  "ConnectionStrings": {
  "DefaultConnection": "Data Source=SQL1004.site4now.net;Initial Catalog=db_ac97b2_intertrips;User Id=db_ac97b2_intertrips_admin;Password=" " ;Encrypt=True;TrustServerCertificate=True;"
}
}
