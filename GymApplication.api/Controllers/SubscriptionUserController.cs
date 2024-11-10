using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Email;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Role = GymApplication.Shared.Emuns.Role;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/subscription-user")]
public class SubscriptionUserController : RestController
{
    private readonly ISender _mediator;

    public SubscriptionUserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(200, Type = typeof(Result<PagedResult<SubscriptionUserResponse>>))]
    public async Task<IResult> Get([FromQuery] GetAllSubscriptionUserRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetSubscriptionUserById")]
    [Authorize(Roles = "Admin,Manager,User")]
    [ProducesResponseType(200, Type = typeof(Result<SubscriptionUserResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetSubscriptionUserById(Guid id)
    {
        var request = new GetSubscriptionUserByIdRequest(id); 
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    [HttpGet("{userId:guid}/user-subscriptions" ,Name = "GetSubscriptionUserByUserId")]
    [Authorize(Roles = "Admin,Manager,User")]
    [ProducesResponseType(200, Type = typeof(Result<SubscriptionUserResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetSubscriptionUserByUserId(Guid userId)
    {
        var request = new GetSubscriptionUserByUserIdRequest(userId); 
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    [HttpGet("{userId:guid}/workout-days" ,Name = "GetSubscriptionUserWorkoutDaysByUserId")]
    [Authorize(Roles = "Admin,Manager,User")]
    [ProducesResponseType(200, Type = typeof(Result<WorkoutDayResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetSubscriptionUserWorkoutDaysByUserId(Guid userId)
    {
        var request = new GetAllSubscriptionUserWorkoutDayRequest(userId); 
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(201, Type = typeof(Result<SubscriptionUserResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Post([FromBody] CreateSubscriptionUserRequest request)
    {
        var result = await _mediator.Send(request);
        return !result.IsSuccess 
            ? HandlerFailure(result) 
            : Results.CreatedAtRoute("GetSubscriptionById", new { id = result.Value.Id }, result);
    }
    
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(200, Type = typeof(Result<SubscriptionUserResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Put(Guid id, [FromBody] UpdateSubscriptionUserRequest request)
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
        var request = new DeleteSubscriptionUserRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}