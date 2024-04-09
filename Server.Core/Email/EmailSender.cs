using System.Net;
using System.Net.Mail;

namespace Server.Core.Email;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string senderMail, string senderPassword, string email, string subject, string message)
    {

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(senderMail, senderPassword)
        };

        return client.SendMailAsync(
            new MailMessage(from: senderMail, to: email, subject, message));
    }
}