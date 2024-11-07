using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/managers")]
public class ManagerController : RestController
{
    private readonly ISender _mediator;

    public ManagerController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IResult> Get([FromQuery] GetAllUserRequest request)
    {
        request.Role = Role.Manager;
        var result = await _mediator.Send(request);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IResult> Post(CreateUserRequest request)
    {
        request.Role = Role.Manager;
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.CreatedAtRoute("GetManagerById", new { id = result.Value.Id }, result) 
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetManagerById")]
    [ProducesResponseType(200, Type = typeof(Result<UserResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    [Authorize]
    public async Task<IResult> GetUserById(Guid id)
    {
        var request = new GetUserById(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}