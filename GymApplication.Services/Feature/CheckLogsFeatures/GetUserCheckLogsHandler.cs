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

public sealed class GetUserCheckLogsHandler : IRequestHandler<GetUserCheckLogsRequest, Result<PagedResult<CheckLogsResponse>>>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IMapper _mapper;

    public GetUserCheckLogsHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IMapper mapper)
    {
        _checkLogRepo = checkLogRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<CheckLogsResponse>>> Handle(GetUserCheckLogsRequest request, CancellationToken cancellationToken)
    {
         // Get all logs for the specified UserId
        var logs = _checkLogRepo.GetQueryable()
            .Include(l => l.User)
            .Include(l => l.UserSubscription)
            .ThenInclude(us => us.Subscription)
            .Where(l => l.UserId == request.UserId && l.IsDeleted == false);

        // Filter by CheckStatus if specified
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
        var currentTime = DateTime.UtcNow;
        logs = request.TimeFrame switch
        {
            "Today" => logs.Where(l => l.CreatedAt.Date == currentTime.Date),
            "Yesterday" => logs.Where(l => l.CreatedAt.Date == currentTime.AddDays(-1).Date),
            "ThisWeek" => logs.Where(l => l.CreatedAt >= GetStartOfWeek(currentTime)),
            "ThisMonth" => logs.Where(l => l.CreatedAt.Year == currentTime.Year && l.CreatedAt.Month == currentTime.Month),
            "LastMonth" => logs.Where(l => l.CreatedAt.Year == currentTime.AddMonths(-1).Year && 
                                           l.CreatedAt.Month == currentTime.AddMonths(-1).Month),
            "90days" => logs.Where(l => l.CreatedAt >= currentTime.AddDays(-90)),
            _ => logs // If "All" or any other case, don't apply additional filtering
        };

        // Sorting
        Expression<Func<CheckLog, object>> sortBy = l => l.CreatedAt;

        logs = request.SortOrder switch
        {
            "asc" => logs.OrderBy(sortBy),
            "desc" => logs.OrderByDescending(sortBy),
            _ => logs.OrderBy(sortBy)
        };

        // Pagination
        var pagedLogs = await PagedResult<CheckLog>.CreateAsync(logs, request.CurrentPage, request.PageSize);

        // Map to response model
        var response = _mapper.Map<PagedResult<CheckLogsResponse>>(pagedLogs);

        // Optionally add additional data such as CheckInTime if CheckStatus is CheckOut
        foreach (var checkLog in response.Items)
        {
            if (checkLog.CheckStatus == LogsStatus.CheckOut.ToString() && checkLog.CheckInId.HasValue)
            {
                var checkInLog = pagedLogs.Items.FirstOrDefault(l => l.Id == checkLog.CheckInId.Value);
                checkLog.CheckInTime = checkInLog?.CreatedAt;
            }
        }

        return Result.Success(response);
    }
    
    private DateTime GetStartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}