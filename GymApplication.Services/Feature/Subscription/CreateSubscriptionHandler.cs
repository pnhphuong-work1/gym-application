using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;
using UUIDNext;

namespace GymApplication.Services.Feature.Subscription;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionRequest, Result<SubscriptionResponse>>
{
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionHandler(IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository, IMapper mapper,
        ICacheServices cacheServices, IRepoBase<DayGroup, Guid> dayGroupRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
        _dayGroupRepository = dayGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionRequest request,
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
            return Result.Failure<SubscriptionResponse>(error);
        }
        var subscription = new Repository.Entities.Subscription()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Name = request.Name,
            Price = request.Price,
            TotalWorkoutTime = TimeOnly.Parse(request.TotalWorkoutTime),
            DayGroupId = dayGroup.Id,
            CreatedAt = DateTime.UtcNow.AddHours(7)
        };
        _subscriptionRepository.Add(subscription);
        
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!result)
        {
            Error error = new("500", "Failed to create subscription");
            return Result.Failure<SubscriptionResponse>(error);
        }
        
        var response = _mapper.Map<SubscriptionResponse>(subscription);
        response.Group = dayGroup.Group;

        _cacheServices.SetAsync(subscription.Id.ToString(), response, TimeSpan.FromMinutes(5), cancellationToken).Wait();

        return Result.Success(response);
    }
}