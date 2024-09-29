using System.Security.Claims;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.Auth;

public sealed class RevokeTokenRequestHandler : IRequestHandler<RevokeTokenRequest, Result>
{
    private readonly ICacheServices _cacheServices;
    private readonly IJwtServices _jwtServices;

    public RevokeTokenRequestHandler(ICacheServices cacheServices, IJwtServices jwtServices)
    {
        _cacheServices = cacheServices;
        _jwtServices = jwtServices;
    }

    public async Task<Result> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = _jwtServices.GetPrincipalFromExpiredToken(request.AccessToken!);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            var error = new Error("404", "Invalid token");
            return Result.Failure(error);
        }
        await _cacheServices.RemoveAsync(email, cancellationToken);
        return Result.Success();
    }
}