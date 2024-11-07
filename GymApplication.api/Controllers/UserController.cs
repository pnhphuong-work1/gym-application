using System.Net.Mime;
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
[Route("api/v{version:apiVersion}/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : RestController
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Result<PagedResult<UserResponse>>))]
    public async Task<IResult> Get([FromQuery] GetAllCustomerRequest request)
    {
        var result = await _mediator.Send(request);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetCustomerById")]
    [ProducesResponseType(200, Type = typeof(Result<CustomerResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetUserById(Guid id)
    {
        var request = new GetCustomerById(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Result<UserResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Post(CreateUserRequest request)
    {
        request.Role = Role.User;
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.CreatedAtRoute("GetCustomerById", new { id = result.Value.Id }, result) 
            : HandlerFailure(result);
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [Authorize]
    public async Task<IResult> Put(Guid id, UpdateUserRequest request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    [Authorize]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var request = new DeleteUserRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
}