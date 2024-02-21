using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Insig.ApplicationServices.Services;
using Insig.Common.Infrastructure.Email;

namespace Insig.Infrastructure.Services;

public class MailingService : IMailingService
{
    private readonly EmailSettings _emailSettings;

    public MailingService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendEmail(string email, EmailTemplate emailTemplate)
    {
        await Execute(new List<string> { email }, emailTemplate);
    }

    public async Task SendEmails(List<string> emails, EmailTemplate emailTemplate)
    {
        await Execute(emails, emailTemplate);
    }

    private async Task Execute(List<string> emails, EmailTemplate emailTemplate)
    {
        if (Debugger.IsAttached)
        {
            Debug.WriteLine("=========== Developer mode is enabled ===========");
            Debug.WriteLine("Message would be sent to following emails:");
            emails.ForEach(x => Debug.WriteLine(x));
            Debug.WriteLine("=================================================");
        }
        else
        {
            var message = new MailMessage()
            {
                From = new MailAddress(_emailSettings.Credentials.Name, _emailSettings.DisplayName),
                Subject = emailTemplate.Subject,
                IsBodyHtml = true,
                Body = emailTemplate.Message,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            using (SmtpClient client = new SmtpClient()
            {
                Host = _emailSettings.Host,
                Port = _emailSettings.Port,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_emailSettings.Credentials.Name, _emailSettings.Credentials.Password),
                EnableSsl = true
            })
            {
                emails.ForEach(message.Bcc.Add);

                await client.SendMailAsync(message);
            }
        }
    }
}