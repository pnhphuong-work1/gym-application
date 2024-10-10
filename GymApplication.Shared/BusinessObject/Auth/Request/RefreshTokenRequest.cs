using FluentValidation;
using GymApplication.Shared.BusinessObject.Auth.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("Access token is required");
        
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}

public sealed class RefreshTokenRequest : IRequest<Result<LoginResponse>>
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}