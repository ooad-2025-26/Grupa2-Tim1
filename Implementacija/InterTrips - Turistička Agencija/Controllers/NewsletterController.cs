using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

public class NewsletterController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Subscribe(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email je obavezan.");

        try
        {
            // TVOJI PODACI
            string mojEmail = "amelaobhodas2@gmail.com"; // Unesi svoj pravi Gmail ovdje
            string lozinka = "bpryrmvxwohfzodx"; // Tvoj App Password (bez razmaka)

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false; // MORA BITI OVDJE
                smtpClient.Credentials = new NetworkCredential("amelaobhodas2@gmail.com", "bpryrmvxwohfzodx");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(mojEmail, "InterTrips Tim"),
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