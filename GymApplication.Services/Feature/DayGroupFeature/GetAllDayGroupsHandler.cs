using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.BusinessObject.Subscription.Request;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public sealed class GetAllDayGroupsHandler : IRequestHandler<GetAllDayGroupsRequest, Result<PagedResult<DayGroupResponse>>>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public GetAllDayGroupsHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IMapper mapper,
        ICacheServices cacheServices)
    {
        _dayGroupRepository = dayGroupRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<PagedResult<DayGroupResponse>>> Handle(GetAllDayGroupsRequest request,
        CancellationToken cancellationToken)
    {
        var redisKey = $"GetAllDayGroups:{request.CurrentPage}:{request.PageSize}:{request.Search}";

        var cache = await _cacheServices.GetAsync<PagedResult<DayGroupResponse>>(redisKey, cancellationToken);

        if (cache is not null)
        {
            return Result.Success(cache);
        }

        var dayGroups = _dayGroupRepository.GetQueryable()
            .Where(s => s.IsDeleted == false);

        var list = await PagedResult<DayGroup>.CreateAsync(dayGroups,
            request.CurrentPage,
            request.PageSize);

        var response = _mapper.Map<PagedResult<DayGroupResponse>>(list);

        await _cacheServices.SetAsync(redisKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return Result.Success(response);
    }
}