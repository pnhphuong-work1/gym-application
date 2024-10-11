namespace GymApplication.Shared.BusinessObject.Auth.Response;

public sealed class LoginResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? UserId { get; init; }
    public string? Role { get; init; }
    public string? FullName { get; init; }
    public DateTime? AccessTokenExpiration { get; init; }
    public DateTime? RefreshTokenExpiration { get; init; }
}