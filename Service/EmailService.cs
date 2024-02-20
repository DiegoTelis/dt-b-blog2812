using System.Net;
using System.Net.Mail;

namespace Blog.Service;
public class EmailService
{

    public bool Send(
        string toName, string toEmail, string subject, string body, 
        string fromName="Teste inicial", string fromEmail="diego.telis@hotmail.com")
    {
        var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);

        smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true;


        var mail = new MailMessage();
        mail.From =  new MailAddress(fromEmail,fromName);
        mail.To.Add(new MailAddress(toEmail, toName));  // é uma lista, pode adicionar para varios
        mail.Subject = subject;
        mail.Body= body;
        mail.IsBodyHtml = true;


        try
        {
            smtpClient.Send(mail);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
