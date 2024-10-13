using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.Subscription;

public sealed class GetAllSubscriptionsHandler : IRequestHandler<GetAllSubscriptionsRequest, Result<PagedResult<SubscriptionResponse>>>
{
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
    private readonly IMapper _mapper;
    public GetAllSubscriptionsHandler(IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }
    public async Task<Result<PagedResult<SubscriptionResponse>>> Handle(GetAllSubscriptionsRequest request, CancellationToken cancellationToken)
    {
        var subscriptions = _subscriptionRepository.GetQueryable()
            .Include(s => s.DayGroup)
            .Where(s => s.IsDeleted == false);
        Expression<Func<Repository.Entities.Subscription, object>> sortBy = request.SortBy switch
        {
            "name" => s => s.Name,
            "totalWorkoutTime" => s => s.TotalWorkoutTime,
            "price" => s => s.Price,
            "group" => s => s.DayGroup.Group!,
            "createdAt" => s => s.CreatedAt,
        };
        
        subscriptions = request.SortOrder switch
        {
            "asc" => subscriptions.OrderBy(sortBy),
            "desc" => subscriptions.OrderByDescending(sortBy),
            _ => subscriptions.OrderBy(sortBy)
        };
        
        Expression<Func<Repository.Entities.Subscription, bool>> searchBy = request.SearchBy switch
        {
            "name" => s => s.Name.Contains(request.Search!),
            "totalWorkoutTime" => s => s.TotalWorkoutTime.Equals(request.Search!),
            "price" => s => s.Price.Equals(request.Search!),
            "createdAt" => l => l.CreatedAt.ToString().Contains(request.Search!),
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            subscriptions = subscriptions.Where(searchBy);
        }
        
        var list = await PagedResult<Repository.Entities.Subscription>.CreateAsync(subscriptions,
            request.CurrentPage,
            request.PageSize);

        var response = _mapper.Map<PagedResult<SubscriptionResponse>>(list);
        return Result.Success(response);
    }
}