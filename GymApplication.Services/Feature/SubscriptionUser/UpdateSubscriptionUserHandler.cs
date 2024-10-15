namespace GymApplication.Services.Feature.SubscriptionUser;

using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

public sealed class UpdateSubscriptionHandler : IRequestHandler<UpdateSubscriptionUserRequest, Result>
{
    private readonly IRepoBase<Repository.Entities.UserSubscription, Guid> _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubscriptionHandler(IRepoBase<Repository.Entities.UserSubscription, Guid> subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSubscriptionUserRequest request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.Id);

        if (subscription is null)
        {
            var notFoundError = new Error("404", "Subscription of user not found");

            return Result.Failure(notFoundError);
        }

        //Update new data
        subscription.UpdatedAt = DateTime.UtcNow;
        subscription.PaymentPrice = request.PaymentPrice ?? subscription.PaymentPrice;
        subscription.WorkoutSteak = request.WorkoutSteak ?? subscription.WorkoutSteak;
        subscription.LongestWorkoutSteak = request.LongestWorkoutSteak ?? subscription.LongestWorkoutSteak;
        subscription.LastWorkoutDate = request.LastWorkoutDate ?? subscription.LastWorkoutDate;
        subscription.SubscriptionEndDate = request.SubscriptionEndDate ?? subscription.SubscriptionEndDate;
        _subscriptionRepository.Update(subscription);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result) return Result.Success();

        var error = new Error("500", "Update Subscription Failed");
        return Result.Failure<SubscriptionUserResponse>(error);
    }
}