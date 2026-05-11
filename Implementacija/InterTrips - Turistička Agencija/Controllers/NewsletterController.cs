using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

public class NewsletterController : Controller
{
    private readonly IConfiguration _config;

    public NewsletterController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email je obavezan.");

        try
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var appPassword = _config["EmailSettings:AppPassword"];
            var senderName = _config["EmailSettings:SenderName"];

            using (var smtpClient = new SmtpClient(smtpServer))
            {
                smtpClient.Port = port;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, appPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "InterTrips - prijava na newsletter",
                    Body = "<h2>Pozdrav!</h2><p>Zahvaljujemo se na prijavi.</p>", 
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Došlo je do greške pri slanju.");
        }
    }
}