using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Auth.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-29")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : RestController
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("login")]
    public async Task<IResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost("logout")]
    public async Task<IResult> Logout(RevokeTokenRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}