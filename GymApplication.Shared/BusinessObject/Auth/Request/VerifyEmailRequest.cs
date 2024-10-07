using FluentValidation;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class VerifyEmailValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

public sealed class VerifyEmailRequest : IRequest<Result>
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}