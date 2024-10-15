using FluentValidation;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed record GetSubscriptionUserByIdRequest(Guid Id) :  IRequest<Result<SubscriptionUserResponse>>;

public sealed class GetSubscriptionUserByIdValidation : AbstractValidator<GetSubscriptionUserByIdRequest> {
    public GetSubscriptionUserByIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}