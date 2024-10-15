using FluentValidation;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed record GetSubscriptionUserByUserIdRequest(Guid Id) : IRequest<Result<List<SubscriptionUserResponse>>>;

public sealed class GetSubscriptionUserByUserIdValidation : AbstractValidator<GetSubscriptionUserByUserIdRequest> {
    public GetSubscriptionUserByUserIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}