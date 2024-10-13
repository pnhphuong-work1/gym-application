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

    public GetAllDayGroupsHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IMapper mapper)
    {
        _dayGroupRepository = dayGroupRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<DayGroupResponse>>> Handle(GetAllDayGroupsRequest request,
        CancellationToken cancellationToken)
    {
        var dayGroups = _dayGroupRepository.GetQueryable()
            .Where(s => s.IsDeleted == false);
        Expression<Func<DayGroup, object>> sortBy = request.SortBy switch
        {
            "group" => d => d.Group,
            "createdAt" => d => d.CreatedAt,
        };
        
        dayGroups = request.SortOrder switch
        {
            "asc" => dayGroups.OrderBy(sortBy),
            "desc" => dayGroups.OrderByDescending(sortBy),
            _ => dayGroups.OrderBy(sortBy)
        };
        
        Expression<Func<DayGroup, bool>> searchBy = request.SearchBy switch
        {
            "group" => d => d.Group.Contains(request.Search!),
            "createdAt" => l => l.CreatedAt.ToString().Contains(request.Search!),
        };
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            dayGroups = dayGroups.Where(searchBy);
        }

        var list = await PagedResult<DayGroup>.CreateAsync(dayGroups,
            request.CurrentPage,
            request.PageSize);

        var response = _mapper.Map<PagedResult<DayGroupResponse>>(list);
        return Result.Success(response);
    }
}