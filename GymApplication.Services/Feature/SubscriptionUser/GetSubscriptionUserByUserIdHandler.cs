using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.SubscriptionUser;

public class GetSubscriptionUserByUserIdHandler : IRequestHandler<GetSubscriptionUserByUserIdRequest,
    Result<List<SubscriptionUserResponse>>>
{
    private readonly IRepoBase<UserSubscription, Guid> _userSubscriptionRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetSubscriptionUserByUserIdHandler(IRepoBase<UserSubscription, Guid> userSubscriptionRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _userSubscriptionRepo = userSubscriptionRepo;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Result<List<SubscriptionUserResponse>>> Handle(GetSubscriptionUserByUserIdRequest request, CancellationToken cancellationToken)
    {
        var existedUser = await _userManager.FindByIdAsync(request.Id.ToString());
        if (existedUser == null)
        {
            var error = new Error("404", "User Not Found");
            return Result.Failure<List<SubscriptionUserResponse>>(error);
        }
        Expression<Func<UserSubscription, object>>[] includes =
        {
            x => x.Subscription,
            x => x.Subscription.DayGroup
        };
        var subsList = await _userSubscriptionRepo
            .GetByConditionsAsync(x => x.UserId == existedUser.Id, includes);
        if (!subsList.Any())
        {
            var error = new Error("404", "No Subscription Found");
            return Result.Failure<List<SubscriptionUserResponse>>(error);
        }
        var response = _mapper.Map<List<SubscriptionUserResponse>>(subsList);
        return Result.Success(response);
    }
}