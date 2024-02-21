using System.Threading.Tasks;
using Insig.Api.Infrastructure.Attributes;
using Insig.Common.Auth.Lookups;
using Insig.PublishedLanguage.Commands.Notifications;
using Insig.PublishedLanguage.Queries.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insig.Api.Controllers;

[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireOneOfRoles(RoleEnum.Admin)]
    [HttpPost("notifications")]
    public async Task<IActionResult> AddNotification([FromBody] AddNotificationCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [RequireOneOfRoles(RoleEnum.Admin)]
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [RequireOneOfRoles(RoleEnum.Admin)]
    [HttpGet("notifications/pending/count")]
    public async Task<IActionResult> GetPendingNotificationsCount([FromQuery] GetPendingNotificationsCountQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [RequireOneOfRoles(RoleEnum.Admin)]
    [HttpPatch("notifications/{id}/completed")]
    public async Task<IActionResult> MarkNotificationAsCompleted([FromRoute] MarkNotificationAsCompletedCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
