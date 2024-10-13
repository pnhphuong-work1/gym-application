using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.Subscription
{
    public class GetSubscriptionByIdHandler : IRequestHandler<GetSubscriptionById, Result<SubscriptionResponse>>
    {
        private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
        private readonly IMapper _mapper;
        public GetSubscriptionByIdHandler(IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository, IMapper mapper, ICacheServices cacheServices)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }
        public async Task<Result<SubscriptionResponse>> Handle(GetSubscriptionById request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(request.Id, null);
            if (subscription != null)
            {
                var subscriptionResponse = _mapper.Map<SubscriptionResponse>(subscription);
                return Result.Success(subscriptionResponse);
            }
            Error error = new("404", "Subscription not found");
            return Result.Failure<SubscriptionResponse>(error);
        }
    }
}