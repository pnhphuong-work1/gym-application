﻿using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.SubscriptionUser;

public class GetAllSubscriptionUserHandler 
    : IRequestHandler<GetAllSubscriptionUserRequest, Result<PagedResult<SubscriptionUserResponse>>>
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepo;
    private readonly IMapper _mapper;

    public GetAllSubscriptionUserHandler(IUserSubscriptionRepository userSubscriptionRepo, IMapper mapper)
    {
        _userSubscriptionRepo = userSubscriptionRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<SubscriptionUserResponse>>> Handle(GetAllSubscriptionUserRequest request, CancellationToken cancellationToken)
    {
        var subscriptions = _userSubscriptionRepo.GetQueryable()
            .Include(su => su.Subscription)
                .ThenInclude(sub => sub.DayGroup)
            .Include(su => su.Payment)
            .Include(su => su.User)
            .Where(s => s.IsDeleted == false);
        Expression<Func<UserSubscription, object>> sortBy = request.SortBy switch
        {
            "name" => s => s.Subscription.Name,
            "paymentPrice" => s => s.PaymentPrice,
            "user" => s => s.User.UserName,
            "createdAt" => s => s.CreatedAt,
        };
        
        subscriptions = request.SortOrder switch
        {
            "asc" => subscriptions.OrderBy(sortBy),
            "desc" => subscriptions.OrderByDescending(sortBy),
            _ => subscriptions.OrderBy(sortBy)
        };
        
        Expression<Func<UserSubscription, bool>> searchBy = request.SearchBy switch
        {
            "userName" => s => s.User.UserName.Contains(request.Search!),
            "paymentPrice" => s => s.PaymentPrice.Equals(request.Search!),
            "createdAt" => l => l.CreatedAt.ToString().Contains(request.Search!),
            "exactUserId" => s => s.UserId.ToString() == request.Search,
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            subscriptions = subscriptions.Where(searchBy);
            if (request.SearchBy == "exactUserId" && !subscriptions.Any())
            {
                var error = new Error("404", "User Not Found");
                return Result.Failure<PagedResult<SubscriptionUserResponse>>(error);
            }
        }
        
        var list = await PagedResult<UserSubscription>.CreateAsync(subscriptions,
            request.CurrentPage,
            request.PageSize);
        var response = _mapper.Map<PagedResult<SubscriptionUserResponse>>(list);
        return Result.Success(response);
    }
}