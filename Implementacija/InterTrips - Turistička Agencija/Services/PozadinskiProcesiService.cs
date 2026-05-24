using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InterTrips___Turistička_Agencija.Services
{
    // FUNKCIONALNOST 3: Asinhrona obrada i red čekanja
    public class PozadinskiProcesiService : BackgroundService
    {
        // Red čekanja u koji stavljamo ID-eve rezervacija koje treba obraditi asinhrono
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
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailAndDocumentService>();
                        await emailService.PosaljiEmailSaLogomAsync(
                            "klijent@primjer.com",
                            "Potvrda Rezervacije #" + rezervacijaId,
                            "<p>Vaša rezervacija je uspješno kreirana i procesirana asinhrono!</p>",
                            rezervacijaId,
                            "Potvrda"
                        );
                    }
                }

                 if (DateTime.Now.Hour == 0) 
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Povucite vaš DB Context preko scope-a i pošaljite podsjetnike za putovanja za 3 dana
                        // Primjer: _db.Rezervacije.Where(r => r.DatumPolaska.Date == DateTime.Today.AddDays(3))
                    }
                }

                await Task.Delay(5000, stoppingToken); // Provjera reda svakih 5 sekundi
            }
        }
    }
}