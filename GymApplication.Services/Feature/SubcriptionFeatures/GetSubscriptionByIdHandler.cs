using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.SubcriptionFeatures
{
    public class GetSubscriptionByIdHandler : IRequestHandler<GetSubscriptionById, Result<SubscriptionResponse>>
    {
        private readonly IRepoBase<Subscription, Guid> _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly ICacheServices _cacheServices;

        public GetSubscriptionByIdHandler(IRepoBase<Subscription, Guid> subscriptionRepository, IMapper mapper, ICacheServices cacheServices)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _cacheServices = cacheServices;
        }

        public async Task<Result<SubscriptionResponse>> Handle(GetSubscriptionById request, CancellationToken cancellationToken)
        {
            var cacheValue = await _cacheServices.GetAsync<SubscriptionResponse>(request.Id.ToString(), cancellationToken);
            if (cacheValue != null) return Result.Success(cacheValue);

            var subscription = await _subscriptionRepository.GetByIdAsync(request.Id, new Expression<Func<Subscription, object>>[0]);
            if (subscription != null)
            {
                var subscriptionResponse = _mapper.Map<SubscriptionResponse>(subscription);
                await _cacheServices.SetAsync(request.Id.ToString(), subscriptionResponse, TimeSpan.FromMinutes(5), cancellationToken);
                return Result.Success(subscriptionResponse);
            }

            Error error = new("404", "Subscription not found");
            return Result.Failure<SubscriptionResponse>(error);
        }
    }
}