using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
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
                    poruka.From = new MailAddress("vaš-gmail@gmail.com", "InterTrips Agencija");
                    poruka.Subject = naslov;
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

        public byte[] GenerisiPdfDokument(string naslovDokumenta, string detalji)
        {
            string htmlSadrzaj = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Arial', sans-serif; padding: 30px; color: #333; }}
                        .header {{ text-align: center; border-bottom: 2px solid #6abfc6; padding-bottom: 10px; }}
                        .content {{ margin-top: 20px; line-height: 1.6; }}
                        .footer {{ margin-top: 50px; text-align: center; font-size: 12px; color: #777; }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>InterTrips Turisticka Agencija</h1>
                        <h3>{naslovDokumenta}</h3>
                    </div>
                    <div class='content'>
                        {detalji}
                    </div>
                    <div class='footer'>
                        Hvala Vam na povjerenju! Vaš InterTrips Tim.
                    </div>
                </body>
                </html>";

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    writer.Write(htmlSadrzaj);
                    writer.Flush();
                    return ms.ToArray();
                }
            }
        }
    }
}