using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Services;
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

                var ciljaniDatum = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

                var rezervacijeZaPodsjetnik = await context.Rezervacije
                    .Include(r => r.Korisnik)
                    .Include(r => r.Paket)
                    .Where(r => r.DatumPolaska == ciljaniDatum)
                    .ToListAsync(stoppingToken);

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

                    string naslov = $"Podsjetnik za putovanje: {nazivPaketa}";
                    string poruka = $@"
                        <h3>Poštovani,</h3>
                        <p>Vaše putovanje na destinaciju <strong>{nazivPaketa}</strong> počinje za 3 dana ({rezervacija.DatumPolaska:dd.MM.yyyy}).</p>
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

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}