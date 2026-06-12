# ✈️ InterTrips - Turistička Agencija

**Sistem za upravljanje turističkom agencijom koji omogućava rezervaciju putovanja, upravljanje paketima i finansijama te generisanje planova putovanja.**

<p align="center">
  <img src="https://github.com/user-attachments/assets/d2d97bfc-5d9e-4676-845d-1460f6837ef3" alt="InterTrips Logo" width="50%">
</p>

---

## O projektu

**InterTrips** je web aplikacija namijenjena upravljanju poslovnim procesima turističke agencije. Sistem omogućava pregled i rezervaciju turističkih paketa, upravljanje terminima putovanja, unos podataka putnika, online plaćanje, administraciju sadržaja i pregled vlastitih rezervacija kroz korisničke panele.

Aplikacija je razvijena kao projektni zadatak i obuhvata funkcionalnosti za **klijente**, **turističke agente** i **administratore**.


---
## Deployment aplikacije

Aplikacija je dostupna na sljedećoj adresi:

🌐 **InterTrips Web Aplikacija**  
http://aobhodas2-001-site1.site4future.com/

---


## Uloge korisnika

Sistem podržava sljedeće korisničke uloge:

- **Administrator** – puni pristup admin panelu i upravljanje sistemom
- **Turistički agent** – upravljanje ponudom, paketima i rezervacijama
- **Klijent / turist** – pregled ponude, rezervacija i plaćanja

---

## Testni podaci za prijavu


| Uloga | E-mail adresa | Lozinka | Opis |
| --- | --- | --- | --- |
| Administrator | `admin@intertrips.ba` | `admin123` | Pristup admin panelu i upravljanje sistemom |
| Turistički agent | `agent@intertrips.ba` | `agent123` | Upravljanje turističkim paketima i ponudom |
| Klijent / turist | `test@intertrips.ba` | `password123` | Kreiranje i pregled rezervacija |

---



## Članovi tima

Projekt je razvijen od strane sljedećih članova tima:

* Amela Obhođaš
* Hamza Jašarević
* Zerina Avdušinović
* Benjamin Bikić

---
##  Konekcijski string

Za povezivanje sa bazom podataka koristi se sljedeći connection string:

```json

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SQL1004.site4now.net;Initial Catalog=db_ac97b2_intertrips;User Id=db_ac97b2_intertrips_admin;Password=" ";Encrypt=True;TrustServerCertificate=True;"
  }


