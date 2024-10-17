using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.SubscriptionUser;

public class GetSubscriptionUserByIdHandler : IRequestHandler<GetSubscriptionUserByIdRequest, Result<SubscriptionUserResponse>?>
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepo;
    private readonly IMapper _mapper;
    

    public GetSubscriptionUserByIdHandler(IUserSubscriptionRepository userSubscriptionRepo, IMapper mapper)
    {
        _userSubscriptionRepo = userSubscriptionRepo;
        _mapper = mapper;
    }
    public async Task<Result<SubscriptionUserResponse>?> Handle(GetSubscriptionUserByIdRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<UserSubscription, object>>[] includes =
        {
            u => u.Subscription,
            u => u.Payment,
            u => u.Subscription.DayGroup
        };
        var subscription = await _userSubscriptionRepo.GetByIdAsync(request.Id, includes);
        if (subscription != null)
        {
            var response = _mapper.Map<SubscriptionUserResponse>(subscription);
            return Result.Success(response);
        }
        Error error = new("404", "Subscription not found");
        return Result.Failure<SubscriptionUserResponse>(error);
    }
}