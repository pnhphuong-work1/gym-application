using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.SubscriptionUser;

public class GetSubscriptionUserByIdHandler : IRequestHandler<GetSubscriptionUserByIdRequest, Result<SubscriptionUserResponse>?>
{
    private readonly IRepoBase<UserSubscription, Guid> _userSubscriptionRepo;

    public GetSubscriptionUserByIdHandler(IRepoBase<UserSubscription, Guid> userSubscriptionRepo)
    {
        _userSubscriptionRepo = userSubscriptionRepo;
    }
    public async Task<Result<SubscriptionUserResponse>?> Handle(GetSubscriptionUserByIdRequest request, CancellationToken cancellationToken)
    {
        var subscription = await _userSubscriptionRepo.GetByIdAsync(request.Id, null);
        if (subscription != null)
        {
            var response = new SubscriptionUserResponse()
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                PaymentId = subscription.PaymentId,
                SubscriptionId = subscription.SubscriptionId,
                PaymentPrice = subscription.PaymentPrice,
                WorkoutSteak = subscription.WorkoutSteak,
                LongestWorkoutSteak = subscription.LongestWorkoutSteak,
                LastWorkoutDate = subscription.LastWorkoutDate,
                SubscriptionEndDate = subscription.SubscriptionEndDate
            };
            return Result.Success(response);
        }
        Error error = new("404", "Subscription not found");
        return Result.Failure<SubscriptionUserResponse>(error);
    }
}