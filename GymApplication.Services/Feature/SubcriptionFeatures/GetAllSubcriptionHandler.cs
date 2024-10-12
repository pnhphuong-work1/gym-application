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

namespace GymApplication.Services.Feature.SubcriptionFeatures;

public sealed class GetAllSubscriptionsHandler : IRequestHandler<GetAllSubscriptionsRequest, Result<PagedResult<SubscriptionResponse>>>
{
    private readonly IRepoBase<Subscription, Guid> _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public GetAllSubscriptionsHandler(IRepoBase<Subscription, Guid> subscriptionRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<PagedResult<SubscriptionResponse>>> Handle(GetAllSubscriptionsRequest request, CancellationToken cancellationToken)
    {
        var redisKey = $"GetAllSubscriptions:{request.CurrentPage}:{request.PageSize}:{request.SortBy}:{request.SortOrder}:{request.Search}";

        var cache = await _cacheServices.GetAsync<PagedResult<SubscriptionResponse>>(redisKey, cancellationToken);

        if (cache is not null)
        {
            return Result.Success(cache);
        }

        var subscriptions = _subscriptionRepository.GetQueryable()
            .Where(s => s.IsDeleted == false);
        Expression<Func<Subscription, object>> sortBy = request.SortBy switch
        {
            "name" => s => s.Name!,
            "totalWorkoutTime" => s => s.TotalWorkoutTime!,
            "price" => s => s.Price!,
        };
        subscriptions = request.SortOrder switch
        {
            "asc" => subscriptions.OrderBy(sortBy),
            "desc" => subscriptions.OrderByDescending(sortBy),
            _ => subscriptions.OrderBy(sortBy)
        };
        
        Expression<Func<Subscription, bool>> searchBy = request.SearchBy switch
        {
            "name" => s => s.Name!.Contains(request.Search!),
            "price" => s => s.Price!.Equals(request.Search!),
            "totalWorkoutTime" => s => s.TotalWorkoutTime!.Equals(request.Search!),
        };
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            subscriptions = subscriptions.Where(searchBy);
        }
        
        var list = await PagedResult<Subscription>.CreateAsync(subscriptions,
            request.CurrentPage,
            request.PageSize);

        var response = _mapper.Map<PagedResult<SubscriptionResponse>>(list);
        
        await _cacheServices.SetAsync(redisKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return Result.Success(response);
    }
}