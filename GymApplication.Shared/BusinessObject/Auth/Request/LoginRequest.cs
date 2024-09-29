using FluentValidation;
using GymApplication.Shared.BusinessObject.Auth.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public sealed class LoginRequest : IRequest<Result<LoginResponse>>
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}