using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.SubscriptionUser;

public sealed class DeleteSubscriptionUserHandler : IRequestHandler<DeleteSubscriptionUserRequest, Result>
{
    private readonly IRepoBase<Repository.Entities.UserSubscription, Guid> _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionUserHandler(IRepoBase<Repository.Entities.UserSubscription, Guid> subscriptionRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSubscriptionUserRequest request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.Id, null);

        if (subscription is null)
        {
            var notFoundError = new Error("404", "Subscription not found");

            return Result.Failure(notFoundError);
        }

        _subscriptionRepository.Delete(subscription);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        if (result) return Result.Success();

        var error = new Error("500", "Delete Subscription Failed");
        return Result.Failure<SubscriptionUserResponse>(error);
    }
}