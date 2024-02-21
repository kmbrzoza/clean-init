using MediatR;

namespace Insig.PublishedLanguage.Commands.Emails;

public class SendEmailCommand : IRequest
{
    public string Email { get; set; }
}
