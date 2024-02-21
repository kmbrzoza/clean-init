using System.Threading.Tasks;
using Insig.PublishedLanguage.Commands.Emails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insig.Api.Controllers;

[ApiController]
public class EmailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("e-mails")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
