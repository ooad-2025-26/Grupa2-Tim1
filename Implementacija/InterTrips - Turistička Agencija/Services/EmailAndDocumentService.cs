using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Services
{
    public class EmailAndDocumentService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public EmailAndDocumentService(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<bool> PosaljiEmailSaLogomAsync(string primalac, string naslov, string sadrzaj, int? rezervacijaId, string tip)
        {
            var log = new LogNotifikacija
            {
                RezervacijaId = rezervacijaId,
                EmailPrimaoca = primalac,
                TipNotifikacije = tip,
                VrijemeSlanja = DateTime.Now,
                Procitana = false
            };

            try
            {
                using var poruka = new MailMessage();

                string senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "intertrips2@gmail.com";
                string appPassword = _configuration["EmailSettings:AppPassword"] ?? "mcslzibvvzoatmnw";

                poruka.To.Add(new MailAddress(primalac));
                poruka.From = new MailAddress(senderEmail, "InterTrips Agencija");
                poruka.Subject = naslov;
                poruka.Body = sadrzaj;
                poruka.IsBodyHtml = true;

                using var klijent = new SmtpClient
                {
                    Host = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com",
                    Port = int.TryParse(_configuration["EmailSettings:Port"], out var p) ? p : 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await klijent.SendMailAsync(poruka);

                log.Status = "Uspjesno";
                log.DatumProcitano = null;
                log.PorukaGreske = string.Empty;

                _db.Add(log);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Status = "Greska";
                log.PorukaGreske = ex.Message;
                log.DatumProcitano = null;

                _db.Add(log);
                await _db.SaveChangesAsync();
                return false;
            }
        }

        public byte[] GenerisiPdfDokument(string naslovDokumenta, Rezervacija rezervacija)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var placanje = _db.Placanja.FirstOrDefault(p => p.RezervacijaId == rezervacija.Id);
            var korisnik = _db.Users.FirstOrDefault(u => u.Id == rezervacija.KorisnikId.ToString());

            string brojRezervacije = $"IT-{rezervacija.Id:D4}-{rezervacija.DatumPolaska.Year}";
            string datumIzdavanja = DateTime.Now.ToString("dd.MM.yyyy.");
            string statusAranzmana = placanje != null ? "PLAĆENO (Potvrđeno)" : "ČEKA SE UPLATA";
            string nazivPaketa = rezervacija.Paket?.Naziv ?? "Nije specificirano";
            string nazivHotela = rezervacija.Paket?.Hotel?.Naziv ?? "Uključen smještaj po programu";
            string periodPutovanja = $"{rezervacija.DatumPolaska:dd.MM.yyyy} - {rezervacija.DatumPovratka:dd.MM.yyyy}";
            string prevozIspis = "Autobuski prevoz uključen";
            if (rezervacija.Paket != null)
            {
                if (rezervacija.Paket.DostupniPrevoz == Enums.VrstaPrevoza.SamoAvion)
                    prevozIspis = "Avionski prevoz uključen";
                else if (rezervacija.Paket.DostupniPrevoz == Enums.VrstaPrevoza.Oboje)
                    prevozIspis = "Kombinovani prevoz (Avion / Autobus)";
            }

            string ukupnaCijena = placanje != null ? $"{placanje.Iznos:F2} BAM" : "Na upit";
            int brojPutnika = rezervacija.Putnici?.Count ?? 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10).FontColor("#1c333b"));

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("InterTrips").FontSize(20).Bold().FontColor("#16343b");
                                col.Item().Text("Turistička agencija").FontSize(9).FontColor("#6f8a90");
                            });

                            row.ConstantItem(220).AlignRight().Column(col =>
                            {
                                col.Item().Text("POTVRDA REZERVACIJE I ITINERER")
                                    .FontSize(11).SemiBold().FontColor("#2aa9b0")
                                    .AlignRight();
                            });
                        });

                        header.Item().PaddingTop(10).LineHorizontal(1).LineColor("#dbe7ea");
                    });

                    page.Content().PaddingTop(14).Column(content =>
                    {
                        content.Item().Row(row =>
                        {
                            row.RelativeItem().Background("#f7faf9").BorderLeft(3).BorderColor("#2aa9b0").Padding(12).Column(col =>
                            {
                                col.Item().Text("Broj rezervacije").FontSize(8).FontColor("#6f8a90").SemiBold();
                                col.Item().Text(brojRezervacije).FontSize(13).Bold().FontColor("#16343b");
                            });

                            row.ConstantItem(14);

                            row.RelativeItem().Background("#f7faf9").BorderLeft(3).BorderColor("#1e7f88").Padding(12).Column(col =>
                            {
                                col.Item().Text("Datum izdavanja").FontSize(8).FontColor("#6f8a90").SemiBold();
                                col.Item().Text(datumIzdavanja).FontSize(13).Bold().FontColor("#16343b");
                            });

                            row.ConstantItem(14);

                            row.RelativeItem().Background("#f7faf9").BorderLeft(3).BorderColor(placanje != null ? "#10b981" : "#f59e0b").Padding(12).Column(col =>
                            {
                                col.Item().Text("Status aranžmana").FontSize(8).FontColor("#6f8a90").SemiBold();
                                col.Item().Text(statusAranzmana).FontSize(12).Bold().FontColor(placanje != null ? "#10b981" : "#f59e0b");
                            });
                        });

                        content.Item().PaddingTop(18).Text("Detalji planiranog putovanja")
                            .FontSize(13).Bold().FontColor("#16343b");

                        content.Item().PaddingTop(8).LineHorizontal(1).LineColor("#dbe7ea");

                        content.Item().PaddingTop(12).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().PaddingRight(8).PaddingBottom(8).Background("#ffffff").Border(1).BorderColor("#e3ecee").Padding(10).Column(col =>
                            {
                                col.Item().Text("Odabrana destinacija / paket").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text(nazivPaketa).FontSize(11).Bold().FontColor("#16343b");
                            });

                            table.Cell().PaddingLeft(8).PaddingBottom(8).Background("#ffffff").Border(1).BorderColor("#e3ecee").Padding(10).Column(col =>
                            {
                                col.Item().Text("Period putovanja").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text(periodPutovanja).FontSize(11).Bold().FontColor("#16343b");
                            });

                            table.Cell().PaddingRight(8).PaddingBottom(8).Background("#ffffff").Border(1).BorderColor("#e3ecee").Padding(10).Column(col =>
                            {
                                col.Item().Text("Smještajni objekat").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text(nazivHotela).FontSize(11).Bold().FontColor("#16343b");
                            });

                            table.Cell().PaddingLeft(8).PaddingBottom(8).Background("#ffffff").Border(1).BorderColor("#e3ecee").Padding(10).Column(col =>
                            {
                                col.Item().Text("Planirani prevoz").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text(prevozIspis).FontSize(11).Bold().FontColor("#16343b");
                            });

                            table.Cell().PaddingRight(8).PaddingBottom(8).Background("#ffffff").Border(1).BorderColor("#e3ecee").Padding(10).Column(col =>
                            {
                                col.Item().Text("Ukupan broj prijavljenih putnika").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text($"{brojPutnika} putnika").FontSize(11).Bold().FontColor("#16343b");
                            });

                            table.Cell().PaddingLeft(8).PaddingBottom(8).Background("#f8fbfc").Border(1).BorderColor("#dbe7ea").Padding(10).Column(col =>
                            {
                                col.Item().Text("Finansijski saldo").FontSize(8).FontColor("#7a9297").SemiBold();
                                col.Item().PaddingTop(3).Text(ukupnaCijena).FontSize(11).Bold().FontColor("#1e7f88");
                            });
                        });

                        content.Item().PaddingTop(18).Text("Manifest / Podaci o putnicima")
                            .FontSize(13).Bold().FontColor("#16343b");

                        content.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#16343b").Padding(6).AlignCenter().Text("#").FontColor(Colors.White).SemiBold().FontSize(8);
                                header.Cell().Background("#16343b").Padding(6).Text("Ime i prezime").FontColor(Colors.White).SemiBold().FontSize(8);
                                header.Cell().Background("#16343b").Padding(6).Text("Datum rođenja").FontColor(Colors.White).SemiBold().FontSize(8);
                                header.Cell().Background("#16343b").Padding(6).Text("Broj pasoša").FontColor(Colors.White).SemiBold().FontSize(8);
                                header.Cell().Background("#16343b").Padding(6).Text("Državljanstvo").FontColor(Colors.White).SemiBold().FontSize(8);
                                header.Cell().Background("#16343b").Padding(6).Text("Kontakt telefon").FontColor(Colors.White).SemiBold().FontSize(8);
                            });

                            if (rezervacija.Putnici != null && rezervacija.Putnici.Any())
                            {
                                int rb = 1;
                                foreach (var p in rezervacija.Putnici)
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).AlignCenter().Text(rb.ToString()).FontSize(8);
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).Text($"{p.Ime} {p.Prezime}").FontSize(9).Bold();
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).Text(p.DatumRodjenja.HasValue ? p.DatumRodjenja.Value.ToString("dd.MM.yyyy.") : "—").FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).Text(p.BrojPasosa ?? "—").FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).Text(p.Drzavljanstvo ?? "—").FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#e8eff1").Padding(6).Text(p.Telefon ?? "—").FontSize(9);
                                    rb++;
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(6).Padding(10).AlignCenter().Text("Nema registrovanih putnika za ovu rezervaciju.")
                                    .FontSize(9).FontColor("#93a7ab");
                            }
                        });
                    });

                    page.Footer().PaddingTop(10).AlignCenter().Text("Ovaj dokument je automatski generisan i validan bez pečata i potpisa.")
                        .FontSize(8)
                        .FontColor("#93a7ab");
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}