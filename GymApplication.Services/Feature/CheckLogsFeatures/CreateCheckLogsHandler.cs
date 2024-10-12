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
        // Check Subscription End date to check if subscription is expired or not 
        var userSubscription = await GetExistSubscription(request);
        
        if (userSubscription is not null)
        {
            return userSubscription;
        }
        
        // Check if the subscription allow user to work out today
        var exsitUserSubscription = await _userSubscriptionRepo.GetByIdAsync(request.UserSubscriptionId);
        List<string> workOutDays = new List<string>();
        if (exsitUserSubscription is not null)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(exsitUserSubscription.SubscriptionId);
            workOutDays = subscription.Name.Split(',').ToList();
        }
        
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

        // Check if the user's subscription allows them to work out today
        if (!workOutDays.Contains(currentDay))
        {
            Error error = new("400", "You are not allowed to work out today");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // Get all the subscription of the user today, then check if the user has already checked in or not
        
        var userTodayLog = await _checkLogRepo.GetByConditionsAsync(x => 
            x.UserId == request.UserId 
            && x.CreatedAt.Date == DateTime.Now.Date 
            && x.IsDeleted == false);
        
        // Check the remaining time
        var remainingTime = await _cacheServices.GetAsync<TimeOnly>($"remaining-time{request.UserId.ToString()}", cancellationToken);

        if (remainingTime < TimeOnly.MinValue)
        {
            
        }

        var userTodayCheckOut = userTodayLog.FirstOrDefault(x => x.CheckStatus == LogsStatus.CheckOut.ToString());
        var userTodayCheckIn = userTodayLog.FirstOrDefault(x => x.CheckStatus == LogsStatus.CheckIn.ToString());
        
        if (userTodayLog.Count > 0 && userTodayCheckOut is not null)
        {
            Error error = new("400", "You have already checked out today");
            return Result.Failure<CheckLogsResponse>(error);
        }

        CheckLog checkLog;
        
        // Create check log
        
        if (userTodayCheckIn is null)
        {
            // 1: For Check-In
            checkLog = new CheckLog()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                UserId = request.UserId,
                UserSubscriptionId = request.UserSubscriptionId,
                CheckStatus = request.CheckStatus,
                WorkoutTime = null,
                CheckInId = null,
                CreatedAt = DateTime.Now
            };
        } else if (userTodayCheckOut is null) 
        {
            checkLog = new CheckLog()
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                UserId = request.UserId,
                UserSubscriptionId = request.UserSubscriptionId,
                CheckStatus = request.CheckStatus,
                WorkoutTime = TimeOnly.FromTimeSpan(userTodayCheckIn.CreatedAt.TimeOfDay - DateTime.Now.TimeOfDay),
                CheckInId = request.CheckInId,
                CreatedAt = DateTime.Now
            };
            var time = (remainingTime - checkLog.WorkoutTime).Value;
            await _cacheServices.SetAsync<TimeOnly>($"remaining-time{request.UserId.ToString()}",TimeOnly.FromTimeSpan(time), cancellationToken);
            
        }
        else
        {
            Error error = new("400", "You have already checked out today");
            return Result.Failure<CheckLogsResponse>(error);
        }
        
        // After Check-Out
        // After checkout, Update the User_Subscription table,
        // Update the LastWorkoutDate to DateTime.Now,
        // Update the WorkoutSteak to +1, Update the LongestWorkoutSteak to +1
        // if the current steak is longer than the previous one

        _checkLogRepo.Add(checkLog);
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