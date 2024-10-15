using FluentValidation;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed class CreateSubscriptionUserValidation : AbstractValidator<CreateSubscriptionUserRequest>
{
    public CreateSubscriptionUserValidation()
    {
        RuleFor(s => s.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be empty");
        RuleFor(s => s.SubscriptionId)
            .NotEqual(Guid.Empty)
            .WithMessage("SubscriptionId cannot be empty");
        RuleFor(s => s.PaymentId)
            .NotEqual(Guid.Empty)
            .WithMessage("PaymentId cannot be empty");
        RuleFor(s => s.PaymentPrice)
            .GreaterThan(0)
            .NotNull()
            .WithMessage("PaymentPrice cannot be null or less than 0");
        RuleFor(s => s.SubscriptionEndDate)
            .NotNull()
            .GreaterThan(DateTime.Today)
            .WithMessage("SubscriptionEndDate cannot be null or today");
    }
}

public sealed class CreateSubscriptionUserRequest : IRequest<Result<SubscriptionUserResponse>>
{
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal PaymentPrice { get; set; }
    public int? WorkoutSteak { get; set; }
    public int? LongestWorkoutSteak { get; set; }
    public DateTime? LastWorkoutDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
}