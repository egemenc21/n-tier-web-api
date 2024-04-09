namespace Server.Core.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string senderMail, string senderPassword,string email, string subject, string message);
}