using FluentValidation;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Payment.Request;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}

public sealed class CreatePaymentRequest : IRequest<Result<PaymentResponse>>
{
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
}