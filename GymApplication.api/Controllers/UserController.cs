using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.User.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/users")]
public class UserController : RestController
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IResult> Get([FromQuery] GetAllUserRequest request)
    {
        var result = await _mediator.Send(request);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<IResult> GetUserById(Guid id)
    {
        var request = new GetUserById(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    public async Task<IResult> Post(CreateUserRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.CreatedAtRoute("GetUserById", new { id = result.Value.Id }, result) 
            : HandlerFailure(result);
    }
}