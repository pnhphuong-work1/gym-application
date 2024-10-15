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

        var dayOfWeek = new List<string>();
        foreach (var sub in allSubs.Value)
        {
            
        }
        return Result.Success<List<WorkoutDayResponse>>(workoutDays);
    }
}