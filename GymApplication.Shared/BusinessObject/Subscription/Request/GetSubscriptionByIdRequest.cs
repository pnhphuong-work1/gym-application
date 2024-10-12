using FluentValidation;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed record GetSubscriptionById(Guid Id) : IRequest<Result<SubscriptionResponse>>;


public sealed class GetSubscriptionByIdValidation : AbstractValidator<GetSubscriptionById> {
    public GetSubscriptionByIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}


