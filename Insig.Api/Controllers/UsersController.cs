using System.Threading.Tasks;
using Insig.PublishedLanguage.Commands.Users;
using Insig.PublishedLanguage.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insig.Api.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("users")]
    public async Task<IActionResult> RegisterUserInDomain([FromBody] RegisterUserCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("users/role")]
    public async Task<IActionResult> GetUserRole([FromQuery] GetUserRoleQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}