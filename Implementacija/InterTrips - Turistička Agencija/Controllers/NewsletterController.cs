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
            var portString = _config["EmailSettings:Port"];
            var port = !string.IsNullOrEmpty(portString) ? int.Parse(portString) : 587;

            var smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var senderName = _config["EmailSettings:SenderName"] ?? "InterTrips";
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var appPassword = _config["EmailSettings:AppPassword"];

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
            {
                return StatusCode(500, "Konfiguracija za email (SenderEmail/AppPassword) nije popunjena u appsettings.json.");
            }

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
                    Body = $@"
<div style='font-family: Arial, sans-serif; color: #333;'>
    <h2>Pozdrav!</h2>
    <p>Zahvaljujemo se na ispunjenoj newsletter formi.</p>
    <hr>
    <p><strong>Zapratite nas:</strong></p>
    <p>Ukoliko imate pitanja ili trebate dodatne informacije, kontaktirajte naš ured:</p>
    <p>
        <strong>InterTrips d.o.o.</strong><br>
        Ferhadija 12<br>
        71000 Sarajevo, BiH<br>
        <a href='https://intertrips.ba' style='color: #2f7d86;'>intertrips.ba</a><br>
        info@intertrips.ba
    </p>
</div>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GREŠKA: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

}