using System.Collections;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.SubscriptionUser;

public class GetAllSubscriptionUserWorkoutDayHandler 
    : IRequestHandler<GetAllSubscriptionUserWorkoutDayRequest, Result<List<WorkoutDayResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISender _sender;

    public GetAllSubscriptionUserWorkoutDayHandler(UserManager<ApplicationUser> userManager, ISender sender)
    {
        _userManager = userManager;
        _sender = sender;
    }

    public async Task<Result<List<WorkoutDayResponse>>> Handle(GetAllSubscriptionUserWorkoutDayRequest request, CancellationToken cancellationToken)
    {
        var workoutDays = new List<WorkoutDayResponse>();
        var existedUser = await _userManager.FindByIdAsync(request.Id.ToString());
        if (existedUser is null)
        {
            var error = new Error("404", "User Not Found");
            return Result.Failure<List<WorkoutDayResponse>>(error);
        }
        //Call a handler that get all subscription of a customer
        var getAllSubsUserParams = new GetSubscriptionUserByUserIdRequest(request.Id);
        var allSubs = await _sender.Send(getAllSubsUserParams);

        
        foreach (var sub in allSubs.Value)
        {
            var targetDays = GetTargetDays(sub.Group);
            workoutDays.AddRange(GetDaysOfWeekInRange(
                sub.SubscriptionStartDate,
                sub.SubscriptionEndDate,
                targetDays,
                sub.Name,
                new TimeOnly(10,10)));
        }
        
        return Result.Success(workoutDays);
    }
    
    //This is a recursion function, use with caution
    private List<WorkoutDayResponse> GetDaysOfWeekInRange(
        DateTime startDate,
        DateTime endDate,
        List<DayOfWeek> targetDays,
        string title,
        TimeOnly workoutTime)
    {
        var workoutDays = new List<WorkoutDayResponse>();
        var current = DateOnly.FromDateTime(startDate);
        //If no more targetDay -> Break 
        if (targetDays.Count == 0)
        {
            return workoutDays;
        }
        //Else continue
        var target = targetDays.First();
        
        while (current.DayOfWeek != target)
        {
            current = current.AddDays(1);
        }
        //After current == targetDay, Add that event and forward to next week
        while (current <= DateOnly.FromDateTime(endDate))
        {
            workoutDays.Add(new()
            {
                Title = title,
                Start = current.ToDateTime(workoutTime),
            });
            current = current.AddDays(7);
        }
        targetDays.Remove(target);
        workoutDays.AddRange(GetDaysOfWeekInRange(startDate, endDate, targetDays, title, workoutTime));
        return workoutDays;
    }

    private List<DayOfWeek> GetTargetDays(string group)
    {
        var targetDays = new List<DayOfWeek>();
        group.Split(", ").ToList().ForEach(
            day =>
            {
                switch (day)
                {
                    case "T2": targetDays.Add(DayOfWeek.Monday);
                        break;
                    case "T3": targetDays.Add(DayOfWeek.Tuesday);
                        break;
                    case "T4": targetDays.Add(DayOfWeek.Wednesday);
                        break;
                    case "T5": targetDays.Add(DayOfWeek.Thursday);
                        break;
                    case "T6": targetDays.Add(DayOfWeek.Friday);
                        break;
                    case "T7": targetDays.Add(DayOfWeek.Saturday);
                        break;
                    case "CN": targetDays.Add(DayOfWeek.Sunday);
                        break;
                    default: break;
                }
            });
        return targetDays;
    }
}