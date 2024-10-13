using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;

namespace GymApplication.Services.Feature.CheckLogsFeatures;

public sealed class GetAllCheckLogsHandler : IRequestHandler<GetAllCheckLogsRequest, Result<PagedResult<CheckLogsResponse>>>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IMapper _mapper;

    public GetAllCheckLogsHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IMapper mapper)
    {
        _checkLogRepo = checkLogRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<CheckLogsResponse>>> Handle(GetAllCheckLogsRequest request, CancellationToken cancellationToken)
    {
        var logs = _checkLogRepo.GetQueryable();
        
        logs.Where(l => l.IsDeleted == false);
        
        Expression<Func<CheckLog, object>> sortBy = request.SortBy switch
        {
            "userName" => l => l.User.UserName,
            "email" => l => l.User.Email,
            "fullName" => l => l.User.FullName,
            "checkStatus" => l => l.CheckStatus!,
            "createdAt" => l => l.CreatedAt,
            _ => l => l.UserId
        };
        
        logs = request.SortOrder switch
        {
            "asc" => logs.OrderBy(sortBy),
            "desc" => logs.OrderByDescending(sortBy),
            _ => logs.OrderBy(sortBy)
        };
        
        Expression<Func<CheckLog, bool>> searchBy = request.SearchBy switch
        {
            "userName" => l => l.User.UserName.Contains(request.Search!),
            "fullName" => l => l.User.FullName.Contains(request.Search!),
            "email" => l => l.User.Email.Contains(request.Search!),
            "createdAt" => l => l.CreatedAt.ToString().Contains(request.Search!),
            _ => l => l.User.UserName.Contains(request.Search!)
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            logs = logs.Where(searchBy);
        }
       
        var list = await PagedResult<CheckLog>.CreateAsync(logs, request.CurrentPage, request.PageSize);
        
        var response = _mapper.Map<PagedResult<CheckLogsResponse>>(list);
        
        return Result.Success(response);
        
    }
}