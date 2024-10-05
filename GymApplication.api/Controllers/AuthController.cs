using System.Net.Mime;
using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Auth.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-29")]
[Route("api/v{version:apiVersion}/auth")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController : RestController
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(200, Type = typeof(Result<LoginResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost("logout")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Logout(RevokeTokenRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(200, Type = typeof(Result<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost("change-password")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(400, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}