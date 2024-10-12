using FluentValidation;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed class CreateSubscriptionRequest : IRequest<Result<SubscriptionResponse>>
{
    public string Name { get; set; }
    public TimeSpan TotalWorkoutTime { get; set; }
    public decimal Price { get; set; }
}
public sealed class CreateSubscriptionRequestValidation : AbstractValidator<CreateSubscriptionRequest> {
    public CreateSubscriptionRequestValidation()
    {
        RuleFor(s => s.Name)
            .NotNull().NotEmpty();

        RuleFor(s => s.Price)
            .GreaterThan(0).NotNull();

        RuleFor(s => s.TotalWorkoutTime)
            .NotNull();
    }
}