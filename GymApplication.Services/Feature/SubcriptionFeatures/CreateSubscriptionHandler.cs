using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;
using UUIDNext;

namespace GymApplication.Services.Feature.SubcriptionFeatures;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionRequest, Result<SubscriptionResponse>>
{
    private readonly IRepoBase<Subscription, Guid> _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public CreateSubscriptionHandler(IRepoBase<Subscription, Guid> subscriptionRepository, IMapper mapper,
        ICacheServices cacheServices)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = new Subscription()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Name = request.Name,
            Price = request.Price,
            TotalWorkoutTime = request.TotalWorkoutTime,
            CreatedAt = DateTime.UtcNow
        };
        _subscriptionRepository.Add(subscription);

        var response = _mapper.Map<SubscriptionResponse>(subscription);

        _cacheServices.SetAsync(subscription.Id.ToString(), response, TimeSpan.FromMinutes(5), cancellationToken);

        return Task.FromResult(Result.Success(response));
    }
}