using FluentValidation;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;


public sealed record GetAllSubscriptionUserWorkoutDayRequest(Guid Id) : IRequest<Result<List<WorkoutDayResponse>>>;

public sealed class GetAllSubscriptionUserWorkoutDayValidation : AbstractValidator<GetAllSubscriptionUserWorkoutDayRequest> {
    public GetAllSubscriptionUserWorkoutDayValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}
