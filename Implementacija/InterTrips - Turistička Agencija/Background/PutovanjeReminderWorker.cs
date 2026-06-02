using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Services;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InterTrips___Turistička_Agencija.Background
{
    public class PutovanjeReminderWorker : BackgroundService
    {
        private readonly IServiceProvider _services;

        public PutovanjeReminderWorker(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();

                var emailService = scope.ServiceProvider.GetRequiredService<EmailAndDocumentService>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var ciljaniDatumPolaska = DateTime.Today.AddDays(3);
                var rezervacijeZaPodsjetnik = context.Rezervacije
                    .Include(r => r.Korisnik)
                    .Include(r => r.Paket)
                    .AsEnumerable()
                    .Where(r => r.DatumPolaska.Date == ciljaniDatumPolaska)
                    .ToList();

                foreach (var rezervacija in rezervacijeZaPodsjetnik)
                {
                    var klijentEmail = rezervacija.Korisnik?.Email;
                    if (string.IsNullOrWhiteSpace(klijentEmail))
                        continue;

                    bool vecPoslato = await context.LogNotifikacija.AnyAsync(l =>
                        l.RezervacijaId == rezervacija.Id &&
                        l.EmailPrimaoca == klijentEmail &&
                        l.TipNotifikacije == "Podsjetnik za putovanje" &&
                        l.Status == "Uspjesno", stoppingToken);

                    if (vecPoslato)
                        continue;

                    var nazivPaketa = rezervacija.Paket?.Naziv ?? "Vaše putovanje";
                    string formatiraniDatum = rezervacija.DatumPolaska.ToString("dd.MM.yyyy");

                    string naslov = $"Podsjetnik za putovanje: {nazivPaketa}";
                    string poruka = $@"
                        <h3>Poštovani,</h3>
                        <p>Vaše putovanje na destinaciju <strong>{nazivPaketa}</strong> počinje za 3 dana ({formatiraniDatum}).</p>
                        <p>Molimo Vas da provjerite Vaše putne dokumente (pasoš, vizu) i plan putovanja.</p>
                        <br>
                        <p>Sretan put želi Vam <strong>InterTrips Agencija</strong>!</p>";

                    await emailService.PosaljiEmailSaLogomAsync(
                        klijentEmail,
                        naslov,
                        poruka,
                        rezervacija.Id,
                        "Podsjetnik za putovanje"
                    );
                }
                var ciljaniDatumPovratka = DateTime.Today.AddDays(-1); 
                var rezervacijeZaRecenziju = context.Rezervacije
                    .Include(r => r.Korisnik)
                    .Include(r => r.Paket)
                    .AsEnumerable()
                    .Where(r => r.DatumPovratka.Date == ciljaniDatumPovratka)
                    .ToList();

                foreach (var rezervacija in rezervacijeZaRecenziju)
                {
                    var klijentEmail = rezervacija.Korisnik?.Email;
                    if (string.IsNullOrWhiteSpace(klijentEmail))
                        continue;

                    bool vecPoslataRecenzija = await context.LogNotifikacija.AnyAsync(l =>
                        l.RezervacijaId == rezervacija.Id &&
                        l.EmailPrimaoca == klijentEmail &&
                        l.TipNotifikacije == "Molba za recenziju" &&
                        l.Status == "Uspjesno", stoppingToken);

                    if (vecPoslataRecenzija)
                        continue;

                    var nazivPaketa = rezervacija.Paket?.Naziv ?? "Vaše putovanje";

                    string naslov = $"Dobrodošli nazad! Kako Vam se svidjelo putovanje u {nazivPaketa}?";
                    string poruka = $@"
                        <h3>Poštovani,</h3>
                        <p>Nadamo se da ste se sretno vratili sa Vašeg putovanja i da nosite prelijepe uspomene iz destinacije <strong>{nazivPaketa}</strong>.</p>
                        <p>Vaše mišljenje nam je izuzetno važno kako bismo poboljšali našu uslugu. Molimo Vas da izdvojite nekoliko trenutaka i ostavite recenziju/povratnu informaciju o aranžmanu, hotelu i cjelokupnom iskustvu.</p>
                        <br>
                        <p>Hvala Vam na ukazanom povjerenju!</p>
                        <p>Srdačan pozdrav,<br><strong>Vaša InterTrips Agencija</strong></p>";

                    await emailService.PosaljiEmailSaLogomAsync(
                        klijentEmail,
                        naslov,
                        poruka,
                        rezervacija.Id,
                        "Molba za recenziju"
                    );
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}