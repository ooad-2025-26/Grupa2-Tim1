# Deseti projektni zadatak

## Paterni ponašanja (Behavioral)

Ova sekcija opisuje planiranu (ciljanu) primjenu odabranih **paterna ponašanja** u sistemu **InterTrips**. Fokus je na tome da se poslovna logika učini modularnom, testabilnom i proširivom, te da se smanji sprega između kontrolera, servisa i domen modela.

> Napomena: Paterni su u ovoj fazi **opisani kao planirana arhitektura** (nije nužno da su svi već implementirani u kodu). Cilj je dokumentovati gdje i zašto bi se pojedini obrazac primijenio u InterTrips-u.

### 1) Strategy (Strategija)

**Problem u kontekstu InterTrips-a:** Formiranje cijene paketa i popusta se često mijenja zavisno od poslovnih pravila (last-minute, popunjenost kapaciteta, akcije, VIP korisnik, kupon). Ako se sva pravila drže u jednoj metodi, nastaje teško održiv i teško testabilan kod.

**Planirana primjena:** Uvodi se interfejs (npr. `IPricingStrategy`) i više konkretnih strategija (npr. `StandardPricingStrategy`, `LastMinutePricingStrategy`, `LowCapacityPricingStrategy`, `CouponPricingStrategy`). Komponenta koja prikazuje ili priprema podatke o paketu (kontroler ili servis) bira strategiju na osnovu uslova (datum polaska, slobodna mjesta, tip korisnika).

**Dobit:** Dodavanje novog pravila cijene znači dodati novu strategiju bez izmjene postojećih; logika cijene je izolovana i testabilna.

---

### 2) State (Stanje)

**Problem u kontekstu InterTrips-a:** Rezervacija i paket imaju stanja (npr. *Kreirana/Potvrđena/Otkazana* za rezervaciju; *Dostupan/Rasprodan* za paket). Ponašanje (dozvoljene akcije i tranzicije) zavisi od trenutnog stanja. Ako se stanje provjerava `if/switch` grananjem kroz više mjesta, lako dolazi do nedosljednosti.

**Planirana primjena:** Uvode se state klase (npr. `IRezervacijaState` sa implementacijama `KreiranaState`, `PotvrdjenaState`, `OtkazanaState`). Rezervacija kao kontekst delegira ponašanje state objektu (npr. `Cancel()`, `Confirm()`, provjera pravila refundacije), a tranzicije stanja se centralizuju.

**Dobit:** Smanjuje se dupliranje provjera; tranzicije su konzistentne i lakše za održavanje.

---

### 3) Template Method (Šablonska metoda)

**Problem u kontekstu InterTrips-a:** Operacije poput kreiranja rezervacije, otkazivanja ili potvrde plaćanja imaju sličan tok: validacija → priprema → transakcija → ažuriranje resursa (kapacitet/sobe/sjedista) → snimanje → notifikacija. Variraju detalji (pravila, dodatni koraci, vrste notifikacija).

**Planirana primjena:** Definiše se apstraktna klasa koja postavlja redoslijed koraka (npr. `ReservationProcessTemplate.Execute()`), dok se varijabilni koraci realizuju kao apstraktne metode/hookovi u podklasama (npr. `Validate()`, `UpdateResources()`, `Notify()`).

**Dobit:** Tok procesa ostaje konzistentan u svim varijantama; lakše dodavanje novih varijanti bez kopiranja koda.

---

### 4) Chain of Responsibility (Lanac odgovornosti)

**Problem u kontekstu InterTrips-a:** Prije rezervacije/otkazivanja/plaćanja postoje brojne nezavisne provjere: autentifikacija, validacija paketa, kapacitet, kupon, rokovi, dostupnost hotela/leta itd. Jedna ogromna metoda postaje nepregledna.

**Planirana primjena:** Kreira se lanac handler-a (npr. `AuthHandler → PaketExistsHandler → CapacityHandler → CouponHandler → PolicyHandler`). Svaki handler obrađuje svoj dio ili prosljeđuje dalje.

**Dobit:** Nove provjere se dodaju kao nove karike; logika validacije je modularna i lako testabilna.

---

### 5) Command (Komanda)

**Problem u kontekstu InterTrips-a:** Poslovne akcije koje mijenjaju sistem (otkazivanje rezervacije, potvrda, primjena kupona, refund) često dodiruju više entiteta i treba ih lako testirati, auditovati i izdvojiti iz kontrolera.

**Planirana primjena:** Uvode se komande (npr. `CancelReservationCommand`, `ApplyCouponCommand`, `ConfirmPaymentCommand`) koje implementiraju npr. `ICommand.Execute()`. Kontroler postaje tanak: validira ulaz i delegira izvršenje komandi.

**Dobit:** Izolovana biznis logika, bolji testovi, mogućnost logovanja/telemetrije; opcionalno i `Undo()` u specifičnim slučajevima.

---

### 6) Iterator (Iterator)

**Problem u kontekstu InterTrips-a:** Postoje kolekcije koje se obilaze (stavke plana putovanja, putnici, usluge). UI i servisi ne bi trebali zavisiti od interne strukture kolekcije.

**Planirana primjena:** Kolekcije se izlažu kroz `IEnumerable` (ili vlastiti iterator kada je potreban poseban redoslijed, filtriranje ili grupisanje). Primjer: iteriranje stavki plana po rednom broju ili po danu.

**Dobit:** Stabilan način obilaska elemenata bez znanja o internoj strukturi; fleksibilno sortiranje/filtriranje.

---

### 7) Mediator (Medijator)

**Problem u kontekstu InterTrips-a:** Rezervacija često uključuje koordinaciju više podsistema: rezervacije, paketi, hoteli, letovi, plaćanja, kuponi, notifikacije. Direktno povezivanje ovih komponenti povećava spregu.

**Planirana primjena:** Uvodi se `ReservationMediator` (ili slična komponenta) koja enkapsulira protokol saradnje i orkestrira tok (provjere → rezervacija resursa → snimanje → pokretanje plaćanja → obavijesti).

**Dobit:** Komponente manje zavise jedna od druge; lakše mijenjanje toka i dodavanje novih koraka.

---

### 8) Observer (Posmatrač)

**Problem u kontekstu InterTrips-a:** Promjene stanja (rezervacija otkazana/potvrđena, plaćanje uspješno/neuspješno, kupon iskorišten) trebaju pokrenuti više reakcija (email, log, analitika, statistika), ali ne želimo da domen logika zna sve potrošače.

**Planirana primjena:** Emitovanje događaja (npr. `ReservationCancelledEvent`) i više subscriber-a/observera (npr. `EmailNotifier`, `AuditLogger`, `AnalyticsUpdater`).

**Dobit:** Dodavanje novih reakcija bez izmjene postojećih procesa; manja sprega i jasnija odgovornost.

---

### 9) Visitor (Posjetilac)

**Problem u kontekstu InterTrips-a:** Potreba za dodavanjem novih operacija nad domen modelima bez mijenjanja njihovih klasa (npr. izvještaji, eksporti, obračun provizije, statistike).

**Planirana primjena:** Domena pruža `Accept(visitor)`, a visitore implementiramo kao zasebne operacije (npr. `RevenueReportVisitor`, `TopDestinationsVisitor`).

**Dobit:** Novi izvještaji/operacije se dodaju bez modifikovanja domen modela; kod ostaje modularan.

---

### 10) Interpreter (Interpreter)

**Problem u kontekstu InterTrips-a:** Pravila cijena i popusta mogu biti složena i često se mijenjati. Hardkodirana pravila otežavaju održavanje i brzu promjenu promocija.

**Planirana primjena:** Definiše se jednostavan “jezik pravila” ili struktura izraza (npr. `daysToDeparture < 7 => -15%`, `freeSeats < 3 => +10%`). Interpreter parsira i evaluira ova pravila nad paketom/rezervacijom.

**Dobit:** Pravila se mogu mijenjati konfiguracijom/bazom bez čestih izmjena koda; jednostavnije A/B testiranje i promocije.

---

### 11) Memento (Memento)

**Problem u kontekstu InterTrips-a:** Kod izmjene planova putovanja (template) ili ključnih entiteta korisno je sačuvati prethodno stanje radi povratka (undo) ili oporavka nakon greške.

**Planirana primjena:** Objekt (originator) kreira memento (snapshot relevantnih polja), a caretaker čuva historiju i omogućava vraćanje. Ovo se može vezati za administrativne izmjene template planova ili kritične izmjene rezervacija.

**Dobit:** Kontrolisan povrat stanja bez izlaganja interne strukture; sigurnije kompleksne izmjene.

---

## Dijagrami komponenti, paketa i raspoređivanja

Potrebno je dizajnirati i detaljno razraditi dijagrame komponenti, paketa i raspoređivanja sistema.
