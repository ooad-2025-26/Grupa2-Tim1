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
                    <meta charset='utf-8' />
                    <style>
                        body {{ 
                            font-family: 'Arial', sans-serif; 
                            margin: 0; 
                            padding: 0; 
                            color: #000000; 
                            font-size: 14px;
                        }}
                        .header {{ 
                            background-color: #2b7a80; 
                            color: #ffffff; 
                            text-align: center; 
                            padding: 20px 10px;
                            margin-bottom: 35px;
                        }}
                        .header h1 {{ 
                            margin: 0; 
                            font-size: 26px; 
                            letter-spacing: 1px;
                        }}
                        .header p {{ 
                            margin: 5px 0 0 0; 
                            font-size: 11px; 
                            opacity: 0.9;
                        }}
                        .main-content {{
                            padding: 0 45px;
                        }}
                        .doc-title {{
                            font-size: 20px;
                            font-weight: bold;
                            margin-bottom: 5px;
                            text-transform: uppercase;
                        }}
                        .doc-meta {{
                            font-size: 12px;
                            color: #333333;
                            line-height: 1.5;
                            margin-bottom: 15px;
                        }}
                        .divider {{
                            border-bottom: 1px solid #c8dcdc;
                            margin-bottom: 25px;
                        }}
                        .grid-container {{
                            width: 100%;
                            margin-bottom: 25px;
                        }}
                        .grid-col {{
                            width: 50%;
                            vertical-align: top;
                        }}
                        .section-title {{
                            font-weight: bold;
                            font-size: 13px;
                            margin-bottom: 12px;
                            text-transform: uppercase;
                        }}
                        .info-line {{
                            font-size: 12px;
                            margin-bottom: 8px;
                        }}
                        .total-amount {{
                            font-size: 16px;
                            font-weight: bold;
                            margin-top: 12px;
                        }}
                        .table-title {{
                            font-weight: bold;
                            font-size: 13px;
                            margin-top: 15px;
                            margin-bottom: 12px;
                            text-transform: uppercase;
                        }}
                        .passengers-table {{
                            width: 100%;
                            border-collapse: collapse;
                            font-size: 11px;
                            margin-bottom: 30px;
                        }}
                        .passengers-table th {{
                            background-color: #f0f8f8;
                            text-align: left;
                            padding: 6px 8px;
                            font-weight: bold;
                        }}
                        .passengers-table td {{
                            padding: 8px;
                            border-bottom: 1px solid #e6f0f0;
                            vertical-align: top;
                        }}
                        .note-line {{
                            font-size: 10px;
                            color: #555555;
                            margin-top: 3px;
                        }}
                        .footer-container {{
                            width: 100%;
                            margin-top: 40px;
                            margin-bottom: 60px;
                        }}
                        .stamp-col {{
                            width: 50%;
                            text-align: left;
                            padding-left: 20px;
                        }}
                        .stamp-circle {{
                            width: 105px;
                            height: 105px;
                            border: 2px solid #2b7a80;
                            border-radius: 50%;
                            display: inline-block;
                            text-align: center;
                            color: #2b7a80;
                            font-size: 9px;
                            font-weight: bold;
                        }}
                        .stamp-text {{
                            margin-top: 28px;
                            line-height: 1.3;
                        }}
                        .signature-col {{
                            width: 50%;
                            text-align: center;
                            vertical-align: bottom;
                            padding-bottom: 15px;
                        }}
                        .signature-line {{
                            width: 180px;
                            border-bottom: 1px solid #000000;
                            margin: 0 auto 8px auto;
                }}
                        .legal-footer {{
                            position: absolute;
                            bottom: 30px;
                            left: 0;
                            right: 0;
                            text-align: center;
                            font-size: 10px;
                            color: #969696;
                            padding: 0 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>INTERTRIPS</h1>
                        <p>Agencija za turizam | Sarajevo, BiH | www.intertrips.ba</p>
                    </div>
                    <div class='main-content'>
                        {detalji}
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