using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.SubcriptionFeatures;

public sealed class UpdateSubscriptionHandler : IRequestHandler<UpdateSubscriptionRequest, Result>
{
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubscriptionHandler(IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository,
         IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.Id);

        if (subscription is null)
        {
            var notFoundError = new Error("404", "Subscription not found");

            return Result.Failure(notFoundError);
        }

        subscription.Name = request.Name;
        subscription.Price = request.Price;
        subscription.TotalWorkoutTime = TimeOnly.Parse(request.TotalWorkoutTime);
        subscription.UpdatedAt = DateTime.Now;

        _subscriptionRepository.Update(subscription);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result) return Result.Success();

        var error = new Error("500", "Update Subscription Failed");
        return Result.Failure<SubscriptionResponse>(error);
    }
}