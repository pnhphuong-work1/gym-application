using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.CheckLogsFeatures;

public sealed class GetCheckLogByIdHandler : IRequestHandler<GetCheckLogsByIdRequest, Result<CheckLogsResponse>>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IMapper _mapper;

    public GetCheckLogByIdHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IMapper mapper)
    {
        _checkLogRepo = checkLogRepo;
        _mapper = mapper;
    }

    public async Task<Result<CheckLogsResponse>> Handle(GetCheckLogsByIdRequest request, CancellationToken cancellationToken)
    {
        var checkLog = await _checkLogRepo.GetByIdAsync(request.Id);
        if (checkLog == null)
        {
            Error error = new("404", "CheckLog not found");
            return Result.Failure<CheckLogsResponse>(error);
        }

        var checkLogResponse = _mapper.Map<CheckLogsResponse>(checkLog);
        return Result.Success(checkLogResponse);
    }
}