using System.Linq.Expressions;
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
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public CreateSubscriptionHandler(IRepoBase<Subscription, Guid> subscriptionRepository, IMapper mapper,
        ICacheServices cacheServices, IRepoBase<DayGroup, Guid> dayGroupRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
        _dayGroupRepository = dayGroupRepository;
    }

    public Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var dayGroup = _dayGroupRepository.GetByConditionsAsync(
            d => d.Group == request.Group,
            new Expression<Func<DayGroup, object>>[0]).
            Result.
            FirstOrDefault(); 
        if (dayGroup == null)
        {
            var error = new Error("DayGroup not found", $"DayGroup '{request.Group}' not found.");
            return Task.FromResult(Result.Failure<SubscriptionResponse>(error));
        }
        var subscription = new Subscription()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Name = request.Name,
            Price = request.Price,
            TotalWorkoutTime = request.TotalWorkoutTime,
            DayGroupId = dayGroup.Id,
            CreatedAt = DateTime.UtcNow
        };
        _subscriptionRepository.Add(subscription);

        var response = _mapper.Map<SubscriptionResponse>(subscription);
        response.Group = dayGroup.Group;

        _cacheServices.SetAsync(subscription.Id.ToString(), response, TimeSpan.FromMinutes(5), cancellationToken).Wait();

        return Task.FromResult(Result.Success(response));
    }
}