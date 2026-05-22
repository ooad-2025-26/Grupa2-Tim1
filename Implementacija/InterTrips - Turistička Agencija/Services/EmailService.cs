using System;
using System.Threading.Tasks;
using InterTrips___Turistička_Agencija.Data;
using InterTrips___Turistička_Agencija.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace InterTrips___Turistička_Agencija.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;

        public EmailService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var emailLog = new LogNotifikacija
            {
                PrimalacEmail = toEmail,
                Naslov = subject,
                DatumSlanja = DateTime.Now,
                PokusajBroj = 1
            };

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("intertrips.ooadofficially@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync("intertrips.ooadofficially@gmail.com", "tvoja-app-lozinka-sa-gmaila");

                await smtp.SendAsync(email);
                emailLog.StatusSlanja = "Uspješno";
            }
            catch (Exception ex)
            {
                emailLog.StatusSlanja = "Greška";
                emailLog.DetaljiGreske = ex.Message;

                try
                {
                    emailLog.PokusajBroj++;
                    await smtp.SendAsync(email);
                    emailLog.StatusSlanja = "Uspješno (Iz drugog pokušaja)";
                }
                catch (Exception retryEx)
                {
                    emailLog.DetaljiGreske += " | Ponovljeni pokušaj propao: " + retryEx.Message;
                }
            }
            finally
            {
                await smtp.DisconnectAsync(true);

                _context.LogoviNotifikacija.Add(emailLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}