using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Services
{
    public class EmailAndDocumentService
    {
        private readonly ApplicationDbContext _db;

        public EmailAndDocumentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> PosaljiEmailSaLogomAsync(string primalac, string naslov, string sadrzaj, int? rezervacijaId, string tip)
        {
            var log = new LogNotifikacija
            {
                RezervacijaId = rezervacijaId,
                EmailPrimaoca = primalac,
                TipNotifikacije = tip,
                VrijemeSlanja = DateTime.Now
            };

            try
            {
                using (var poruka = new MailMessage())
                {
                    poruka.To.Add(new MailAddress(primalac));
                    string posiljalacEmail = _configuration["EmailSettings:SenderEmail"] ?? "intertrips2@gmail.com";
                    poruka.From = new MailAddress(posiljalacEmail, "InterTrips Agencija");
                    poruka.Body = sadrzaj;
                    poruka.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("vaš-gmail@gmail.com", "vaša-lozinka-aplikacije");
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        await smtp.SendMailAsync(poruka);
                    }
                }

                log.Status = "Uspjesno";
                log.Procitana = true;
                log.DatumProcitano = DateTime.Now;
                _db.Add(log);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Status = "Greska";
                log.PorukaGreske = ex.Message;
                _db.Add(log);
                await _db.SaveChangesAsync();
                return false;
            }
        }

        public byte[] GenerisiPdfDokument(string naslovDokumenta, string detaljiHtml)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(header =>
                    {
                        header.Item().Background("#2b7a80").Padding(12).AlignCenter().Text("INTERTRIPS")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.White);

                        header.Item().AlignCenter().Text("Agencija za turizam | Sarajevo, BiH | www.intertrips.ba")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);

                        header.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(15).Column(content =>
                    {
                        content.Item().AlignCenter().Text(naslovDokumenta)
    .FontSize(16)
    .Bold()
    .FontColor("#2b7a80");

                        content.Item().PaddingTop(10).Text(StripHtml(detaljiHtml))
                            .FontSize(10)
                            .FontColor(Colors.Black);
                    });

                    page.Footer().AlignCenter().Text("Ovaj dokument je validan bez pečata i potpisa ukoliko je generisan elektronskim putem.")
                        .FontSize(8)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        private static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sb = new StringBuilder(input);

            sb.Replace("<br />", "\n");
            sb.Replace("<br>", "\n");
            sb.Replace("</div>", "\n");
            sb.Replace("</p>", "\n");
            sb.Replace("</tr>", "\n");
            sb.Replace("</td>", " ");
            sb.Replace("</th>", " ");
            sb.Replace("<li>", "\n- ");
            sb.Replace("</li>", "");
            sb.Replace("</h1>", "\n");
            sb.Replace("</h2>", "\n");
            sb.Replace("</h3>", "\n");
            sb.Replace("</h4>", "\n");

            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "<.*?>", string.Empty)
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Trim();
        }
    }
}