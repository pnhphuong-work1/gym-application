using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public class GetDayGroupByIdHandler : IRequestHandler<GetDayGroupById, Result<DayGroupResponse>>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public GetDayGroupByIdHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _dayGroupRepository = dayGroupRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<DayGroupResponse>> Handle(GetDayGroupById request, CancellationToken cancellationToken)
    {
        var cacheValue = await _cacheServices.GetAsync<DayGroupResponse>(request.Id.ToString(), cancellationToken);
        if (cacheValue != null) return Result.Success(cacheValue);

        var dayGroup = await _dayGroupRepository.GetByIdAsync(request.Id, new Expression<Func<DayGroup, object>>[0]);
        if (dayGroup != null)
        {
            var dayGroupResponse = _mapper.Map<DayGroupResponse>(dayGroup);
            await _cacheServices.SetAsync(request.Id.ToString(), dayGroupResponse, TimeSpan.FromMinutes(5), cancellationToken);
            return Result.Success(dayGroupResponse);
        }

        Error error = new("404", "DayGroup not found");
        return Result.Failure<DayGroupResponse>(error);
    }
}