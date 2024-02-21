using System.Collections.Generic;
using System.Threading.Tasks;
using Insig.Common.Infrastructure.Email;

namespace Insig.ApplicationServices.Services;

public interface IMailingService
{
    Task SendEmail(string email, EmailTemplate emailTemplate);
    Task SendEmails(List<string> emails, EmailTemplate emailTemplate);
}