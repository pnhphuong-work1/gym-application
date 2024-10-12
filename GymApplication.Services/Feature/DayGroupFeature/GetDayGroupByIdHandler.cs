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

    public GetDayGroupByIdHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _dayGroupRepository = dayGroupRepository;
        _mapper = mapper;
    }

    public async Task<Result<DayGroupResponse>> Handle(GetDayGroupById request, CancellationToken cancellationToken)
    {
        var dayGroup = await _dayGroupRepository.GetByIdAsync(request.Id);
        if (dayGroup == null)
        {
            Error error = new("404", "CheckLog not found");
            return Result.Failure<DayGroupResponse>(error);
        }

        var dayGroupResponse = _mapper.Map<DayGroupResponse>(dayGroup);
        return Result.Success(dayGroupResponse);
    }
}