using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.Subscription;

public sealed class DeleteSubscriptionHandler : IRequestHandler<DeleteSubscriptionRequest, Result>
{
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionHandler(IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.Id, null);

        if (subscription is null)
        {
            var notFoundError = new Error("404", "DayGroup not found");

            return Result.Failure(notFoundError);
        }

        _subscriptionRepository.Delete(subscription);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        if (result) return Result.Success();

        var error = new Error("500", "Delete Subscription Faied");
        return Result.Failure<SubscriptionResponse>(error);
    }
}