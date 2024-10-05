using FluentValidation;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");
        
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");
        
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

public sealed class ChangePasswordRequest : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}