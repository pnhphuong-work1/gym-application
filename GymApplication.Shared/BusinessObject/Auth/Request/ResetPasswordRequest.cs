using FluentValidation;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");
        
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
        
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required");
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword)
            .WithMessage("Confirm password must match new password");
    }
}

public sealed class ResetPasswordRequest : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}