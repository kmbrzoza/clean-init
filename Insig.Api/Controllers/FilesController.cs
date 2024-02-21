using System.Threading.Tasks;
using Insig.PublishedLanguage.Commands.Files;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insig.Api.Controllers;

[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("files")]
    public async Task<IActionResult> AddFile([FromForm] AddFileCommand command)
    {
        await _mediator.Send(command);
        return Ok(command.Result);
    }
}
