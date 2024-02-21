using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Services;
using Insig.PublishedLanguage.Commands.Emails;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Emails;

public class SendEmailHandler : IRequestHandler<SendEmailCommand>
{
    private readonly IMailingService _mailingService;

    public SendEmailHandler(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            await _mailingService.SendEmail(
                request.Email,
                new("Subject of example email", "Message of example email")
            );
        }
    }
}
