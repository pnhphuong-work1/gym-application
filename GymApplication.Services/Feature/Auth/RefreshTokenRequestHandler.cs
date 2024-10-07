using System.Security.Claims;
using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Auth.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtServices _jwtServices;
    private readonly ICacheServices _cacheServices;

    public RefreshTokenRequestHandler(UserManager<ApplicationUser> userManager, IJwtServices jwtServices, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _jwtServices = jwtServices;
        _cacheServices = cacheServices;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = _jwtServices.GetPrincipalFromExpiredToken(request.AccessToken!);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            var error = new Error("400", "Invalid token");
            return Result.Failure<LoginResponse>(error);
        }
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsDeleted)
        {
            var error = new Error("404", "User not found");
            return Result.Failure<LoginResponse>(error);
        }
        
        var refreshToken = await _cacheServices.GetAsync<LoginResponse>(email, cancellationToken);
        
        if (refreshToken is null || refreshToken.RefreshToken != request.RefreshToken)
        {
            var error = new Error("400", "Invalid token");
            return Result.Failure<LoginResponse>(error);
        }
        var claims = principal.Claims;
        
        var newRefreshToken = _jwtServices.GenerateRefreshToken();
        
        var loginResponse = new LoginResponse
        {
            AccessToken = _jwtServices.GenerateAccessToken(claims),
            RefreshToken = newRefreshToken,
            AccessTokenExpiration = DateTime.Now.AddMinutes(5),
            RefreshTokenExpiration = DateTime.Now.AddHours(3)
        };
        
        await _cacheServices.SetAsync(email, loginResponse, cancellationToken);
        
        return Result.Success(loginResponse);
    }
}