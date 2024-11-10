using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/subscriptions")]
public class SubscriptionController : RestController
{
    private readonly ISender _mediator;

    public SubscriptionController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,User")]
    [ProducesResponseType(200, Type = typeof(Result<PagedResult<SubscriptionResponse>>))]
    public async Task<IResult> Get([FromQuery] GetAllSubscriptionsRequest request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetSubscriptionById")]
    [Authorize(Roles = "Admin,Manager,User")]
    [ProducesResponseType(200, Type = typeof(Result<SubscriptionResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetSubscriptionById(Guid id)
    {
        var request = new GetSubscriptionById(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(201, Type = typeof(Result<SubscriptionResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Post([FromBody] CreateSubscriptionRequest request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
            ? Results.CreatedAtRoute("GetSubscriptionById", new { id = result.Value.Id }, result)
            : HandlerFailure(result);
    }
    
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(200, Type = typeof(Result<SubscriptionResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Put(Guid id, [FromBody] UpdateSubscriptionRequest request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var request = new DeleteSubscriptionRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}