using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UUIDNext;

namespace GymApplication.Services.Feature.CheckLogsFeatures;

public class CreateCheckLogsHandler : IRequestHandler<CreateCheckLogsRequest, Result<CheckLogsResponse>>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepoBase<UserSubscription, Guid> _userSubscriptionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepo;
    private readonly ICacheServices _cacheServices;

    public CreateCheckLogsHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IMapper mapper, UserManager<ApplicationUser> userManager, IRepoBase<UserSubscription, Guid> userSubscriptionRepo, IUnitOfWork unitOfWork, IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepo, ICacheServices cacheServices)
    {
        _checkLogRepo = checkLogRepo;
        _mapper = mapper;
        _userManager = userManager;
        _userSubscriptionRepo = userSubscriptionRepo;
        _unitOfWork = unitOfWork;
        _subscriptionRepo = subscriptionRepo;
        _cacheServices = cacheServices;
    }

    public async Task<Result<CheckLogsResponse>> Handle(CreateCheckLogsRequest request, CancellationToken cancellationToken)
    {
        // Check existing user
        var user = await GetExistUser(request);
        
        if (user is not null)
        {
            return user;
        }
        
        // Check existing subscription
        if (request.UserSubscriptionId == Guid.Empty)
        {
            Error error = new("400", "User Subscription Id is required");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        Expression<Func<UserSubscription, object>>[] includes =
        [
            x => x.Subscription,
            x => x.Subscription.DayGroup
        ];
        
        var existUserSubscription = await _userSubscriptionRepo.GetByIdAsync(request.UserSubscriptionId, includes); 
        
        if (existUserSubscription is null)
        {
            Error error = new("404", "Subscription not found");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // Check Subscription End date to check if subscription is expired or not 
        if (existUserSubscription.SubscriptionEndDate < DateTime.Now)
        {
            Error error = new("400", "Subscription has expired");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // Check if the subscription allow user to work out today
        var dayGroup = existUserSubscription.Subscription?.DayGroup;

        if (dayGroup is null || string.IsNullOrEmpty(dayGroup.Group))
        {
            Error error = new("400", "Invalid subscription or day group");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        List<string> workOutDays = dayGroup.Group
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(day => day.Trim()) // Trim whitespace around each day
            .ToList();

        
        string currentDay = DateTime.Now.DayOfWeek switch
        {
            DayOfWeek.Monday => "T2",
            DayOfWeek.Tuesday => "T3",
            DayOfWeek.Wednesday => "T4",
            DayOfWeek.Thursday => "T5",
            DayOfWeek.Friday => "T6",
            DayOfWeek.Saturday => "T7",
            DayOfWeek.Sunday => "CN",
            _ => throw new ArgumentOutOfRangeException("Don't know error occur while converting day") // To handle any unexpected values
        };
        
        if (!workOutDays.Contains(currentDay))
        {
            Error error = new("400", "You are not allowed to work out today");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // Check the remaining time
        var remainingTime = await _cacheServices.GetAsync<TimeOnly?>($"remaining-time{request.UserId.ToString()}-{DateTime.Today}", cancellationToken);

        if (remainingTime is not null)
        {
            if (remainingTime < TimeOnly.MinValue)
            {
                Error error = new("400", "You have no remaining time to work out today");
                return Result.Failure<CheckLogsResponse>(error);
            } 
        }
        
        // Check today Log then Create Check log
        var startOfLocalDay = DateTime.UtcNow.Date; // Start of today in GMT+7, converted to UTC
        var endOfLocalDay = startOfLocalDay.AddDays(1).AddTicks(-1); // End of today in GMT+7, converted to UTC

        var userTodayLog = await _checkLogRepo.GetByConditionsAsync(x =>
            x.UserId == request.UserId
            && x.CreatedAt >= startOfLocalDay
            && x.CreatedAt <= endOfLocalDay
            && x.IsDeleted == false);
        
        var lastCheckLog = userTodayLog
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        CheckLog checkLog;
        
        // Create check log
        
        if (lastCheckLog == null)
        {
            // No check-in or check-out exists for today, create a new check-in
            checkLog = new CheckLog()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                UserId = request.UserId,
                UserSubscriptionId = request.UserSubscriptionId,
                CheckStatus = LogsStatus.CheckIn.ToString(), // Assume Check-In as default
                WorkoutTime = null,
                CheckInId = null,
                CreatedAt = DateTime.UtcNow // Convert to GMT+7
                //CreatedAt = DateTime.Now
            };
            
            // Calculate the remaining time until the end of the day (midnight)
            var currentTime = DateTime.Now;
            var endOfDay = currentTime.Date.AddDays(1).AddTicks(-1); // End of the day (23:59:59)
            TimeSpan timeToLive = endOfDay - currentTime;

            // Set the remaining workout time in cache with expiration at the end of the day
            await _cacheServices.SetAsync<TimeOnly>(
                $"remaining-time{request.UserId.ToString()}-{DateTime.Today}",
                (existUserSubscription.Subscription?.TotalWorkoutTime).Value,
                timeToLive,  // TimeSpan for cache expiration
                cancellationToken
            );
            
        }
        else if (lastCheckLog.CheckStatus == LogsStatus.CheckIn.ToString())
        {
            // Last log was a check-in, so create a check-out
            checkLog = new CheckLog()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                UserId = request.UserId,
                UserSubscriptionId = request.UserSubscriptionId,
                CheckStatus = LogsStatus.CheckOut.ToString(),
                WorkoutTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay - lastCheckLog.CreatedAt.TimeOfDay),
                CheckInId = lastCheckLog.Id, // Link to the last check-in
                CreatedAt = DateTime.UtcNow
                //CreatedAt = DateTime.Now
            };

            // Update remaining workout time if necessary
            // Calculate the remaining time until the end of the day (midnight)
            var currentTime = DateTime.Now;
            var endOfDay = currentTime.Date.AddDays(1).AddTicks(-1); // End of the day (23:59:59)
            TimeSpan timeToLive = endOfDay - currentTime;

            // Update remaining workout time and set it in cache with expiration at the end of the day
            var remainingTimeToday = (remainingTime - checkLog.WorkoutTime).Value;
            await _cacheServices.SetAsync<TimeOnly>(
                $"remaining-time{request.UserId.ToString()}-{DateTime.Today}",
                TimeOnly.FromTimeSpan(remainingTimeToday),
                timeToLive,  // TimeSpan for cache expiration
                cancellationToken
            );
        }
        else if (lastCheckLog.CheckStatus == LogsStatus.CheckOut.ToString())
        {
            // Last log was a check-out, so create a new check-in
            checkLog = new CheckLog()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                UserId = request.UserId,
                UserSubscriptionId = request.UserSubscriptionId,
                CheckStatus = LogsStatus.CheckIn.ToString(),
                WorkoutTime = null,
                CheckInId = null,
                CreatedAt = DateTime.UtcNow
            };
        }
        else
        {
            Error error = new("400", "Unexpected status in check logs");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // After Check-Out
        // After checkout, Update the User_Subscription table,
        // Update the LastWorkoutDate to DateTime.Now,
        // Update the WorkoutSteak to +1, Update the LongestWorkoutSteak to +1
        // if the current steak is longer than the previous one
        
        // Note: Update later for the LongestWorkoutSteak because the subscription maybe T2,T4 and T6
        if (checkLog.CheckStatus == LogsStatus.CheckOut.ToString())
        {
            existUserSubscription.LastWorkoutDate = DateTime.UtcNow.AddHours(7);
            if (userTodayLog.Count(x => x.CheckStatus == LogsStatus.CheckOut.ToString()) == 0)
            {
                existUserSubscription.WorkoutSteak += 1;
            }
            if (existUserSubscription.WorkoutSteak > existUserSubscription.LongestWorkoutSteak)
            {
                existUserSubscription.LongestWorkoutSteak = existUserSubscription.WorkoutSteak;
            }
            
            _userSubscriptionRepo.Update(existUserSubscription);
        }

        _checkLogRepo.Add(checkLog);
        // var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        //
        // if (!result)
        // {
        //     Error error = new("500", "Failed to create check log");
        //     return Result.Failure<CheckLogsResponse>(error);CheckLog
        // }
       
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!result) 
        {
            Error error = new("500", "Failed to create check log");
            return Result.Failure<CheckLogsResponse>(error);
        }
            
        var response = _mapper.Map<CheckLogsResponse>(checkLog);

        return Result.Success(response);
    }

    private async Task<Result<CheckLogsResponse>?> GetExistUser(CreateCheckLogsRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            Error error = new("400", "User Id is required");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        var existUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        
        if (existUser is null)
        {
            Error error = new("404", "User not found");
            return Result.Failure<CheckLogsResponse>(error);
        }

        return null;

    }
    
    private async Task<Result<CheckLogsResponse>?> GetExistSubscription(CreateCheckLogsRequest request)
    {
        if (request.UserSubscriptionId == Guid.Empty)
        {
            Error error = new("400", "User Subscription Id is required");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        var existSubscription = await _userSubscriptionRepo.GetByIdAsync(request.UserSubscriptionId, null);
        
        if (existSubscription is null)
        {
            Error error = new("404", "Subscription not found");
            return Result.Failure<CheckLogsResponse>(error);
        }

        if (existSubscription.SubscriptionEndDate < DateTime.Now)
        {
            Error error = new("400", "Subscription has expired");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        return null;

    }
    
    
}