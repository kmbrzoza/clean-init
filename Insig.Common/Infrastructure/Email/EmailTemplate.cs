namespace Insig.Common.Infrastructure.Email;

public class EmailTemplate
{
    public string Subject { get; set; }
    public string Message { get; set; }

    public EmailTemplate(string subject, string message)
    {
        Subject = subject;
        Message = message;
    }
}