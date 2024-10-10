using System.Security.Claims;
using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Auth.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class LoginRequestHandler : IRequestHandler<LoginRequest, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtServices _jwtServices;
    private readonly ICacheServices _cacheServices;

    public LoginRequestHandler(UserManager<ApplicationUser> userManager, IJwtServices jwtServices, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _jwtServices = jwtServices;
        _cacheServices = cacheServices;
    }

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is null || user.IsDeleted)
        {
            var error = new Error("404", "Invalid email or password");
            return Result.Failure<LoginResponse>(error);
        }
        
        var result = await _userManager.CheckPasswordAsync(user, request.Password!);
        
        if (!result)
        {
            var error = new Error("404", "Invalid email or password");
            return Result.Failure<LoginResponse>(error);
        }
        
        var role = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(role.Select(item => new Claim(ClaimTypes.Role, item)));
        
        var accessToken = _jwtServices.GenerateAccessToken(claims);
        var refreshToken = _jwtServices.GenerateRefreshToken();
        
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Role = role.FirstOrDefault(),
            FullName = user.FullName,
            AccessTokenExpiration = DateTime.Now.AddMinutes(5),
            RefreshTokenExpiration = DateTime.Now.AddMinutes(180)
        };
        
        await _cacheServices.SetAsync(request.Email!, response, TimeSpan.FromHours(3), cancellationToken);
        
        return Result.Success(response);
    }
}