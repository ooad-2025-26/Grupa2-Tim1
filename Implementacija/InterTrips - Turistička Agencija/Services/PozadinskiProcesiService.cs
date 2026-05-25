using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InterTrips___Turistička_Agencija.Services
{
    public class PozadinskiProcesiService : BackgroundService
    {
        private static readonly ConcurrentQueue<int> RedZaRezervacije = new ConcurrentQueue<int>();
        private readonly IServiceScopeFactory _scopeFactory;

        public PozadinskiProcesiService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public static void DodajRezervacijuURed(int rezervacijaId)
        {
            RedZaRezervacije.Enqueue(rezervacijaId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (RedZaRezervacije.TryDequeue(out int rezervacijaId))
                {
                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailAndDocumentService>();

                    var rezervacija = await context.Rezervacije
                        .Include(r => r.Korisnik)
                        .Include(r => r.Paket)
                        .FirstOrDefaultAsync(r => r.Id == rezervacijaId, stoppingToken);

                    if (rezervacija?.Korisnik?.Email != null)
                    {
                        string naslov = $"Potvrda rezervacije #{rezervacija.Id}";
                        string poruka = $@"
                            <h3>Poštovani,</h3>
                            <p>Vaša rezervacija za <strong>{rezervacija.Paket?.Naziv ?? "putovanje"}</strong> je uspješno kreirana.</p>
                            <p>Broj rezervacije: <strong>#{rezervacija.Id}</strong></p>
                            <p>Hvala što koristite InterTrips.</p>";

                        await emailService.PosaljiEmailSaLogomAsync(
                            rezervacija.Korisnik.Email,
                            naslov,
                            poruka,
                            rezervacija.Id,
                            "Potvrda rezervacije"
                        );
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}