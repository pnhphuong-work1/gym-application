using FluentValidation;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Auth.Request;

public sealed class ResendVerifyEmailValidator : AbstractValidator<ResendVerifyEmailRequest>
{
    public ResendVerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

public sealed class ResendVerifyEmailRequest : IRequest<Result>
{
    public string Email { get; set; } = null!;
}