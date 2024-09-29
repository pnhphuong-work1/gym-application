using System.Security.Claims;

namespace GymApplication.Services.Abstractions;

public interface IJwtServices
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}