using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.CheckLogsFeatures;

public sealed class GetAllCheckLogsHandler : IRequestHandler<GetAllCheckLogsRequest, Result<PagedResult<CheckLogsResponse>>>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IMapper _mapper;

    public GetAllCheckLogsHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IMapper mapper)
    {
        _checkLogRepo = checkLogRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<CheckLogsResponse>>> Handle(GetAllCheckLogsRequest request, CancellationToken cancellationToken)
    {
        var logs = _checkLogRepo.GetQueryable();
        
        logs = logs.Include(l => l.User)
            .Include(l => l.UserSubscription)
            .ThenInclude(us => us.Subscription)
            .Where(l => l.IsDeleted == false);
        
        // Apply CheckStatus filter
        if (request.CheckStatus != "All")
        {
            var checkStatus = request.CheckStatus switch
            {
                "CheckIn" => LogsStatus.CheckIn.ToString(),
                "CheckOut" => LogsStatus.CheckOut.ToString(),
                _ => LogsStatus.CheckIn.ToString() // Default case, just in case
            };

            logs = logs.Where(l => l.CheckStatus == checkStatus);
        }
        
        // Apply TimeFrame filter
        var currentTime = DateTime.UtcNow;  // Or use DateTime.Now if you're working with local time
        logs = request.TimeFrame switch
        {
            "Today" => logs.Where(l => l.CreatedAt.Date == currentTime.Date),
            "Yesterday" => logs.Where(l => l.CreatedAt.Date == currentTime.AddDays(-1).Date),
            "ThisWeek" => logs.Where(l => l.CreatedAt >= GetStartOfWeek(currentTime)),
            "ThisMonth" => logs.Where(l => l.CreatedAt.Year == currentTime.Year && l.CreatedAt.Month == currentTime.Month),
            "90days" => logs.Where(l => l.CreatedAt >= currentTime.AddDays(-90)),
            _ => logs // If "All" or any other case, don't apply additional filtering
        };
        
        Expression<Func<CheckLog, object>> sortBy = request.SortBy switch
        {
            "fullName" => l => l.User.FullName,
            "createdAt" => l => l.CreatedAt,
            _ => l => l.User.FullName
        };
        
        logs = request.SortOrder switch
        {
            "asc" => logs.OrderBy(sortBy),
            "desc" => logs.OrderByDescending(sortBy),
            _ => logs.OrderBy(sortBy)
        };
        
        Expression<Func<CheckLog, bool>> searchBy = request.SearchBy switch
        {
            "fullName" => l => l.User.FullName.Contains(request.Search!),
            "subscriptionName" => l => l.UserSubscription.Subscription.Name.Contains(request.Search!),
            _ => l => l.User.FullName.Contains(request.Search!)
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            logs = logs.Where(searchBy);
        }

        logs = logs.OrderByDescending(l => l.CreatedAt);
       
        var list = await PagedResult<CheckLog>.CreateAsync(logs, request.CurrentPage, request.PageSize);
        
        var response = _mapper.Map<PagedResult<CheckLogsResponse>>(list);
        
        return Result.Success(response);
        
    }
    
    private DateTime GetStartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
    
}