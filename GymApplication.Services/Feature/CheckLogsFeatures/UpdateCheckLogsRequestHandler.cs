using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.CheckLogsFeatures;

public class UpdateCheckLogsRequestHandler : IRequestHandler<UpdateCheckLogsRequest, Result>
{
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCheckLogsRequestHandler(IRepoBase<CheckLog, Guid> checkLogRepo, IUnitOfWork unitOfWork)
    {
        _checkLogRepo = checkLogRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCheckLogsRequest request, CancellationToken cancellationToken)
    {
        var log = await _checkLogRepo.GetByIdAsync(request.Id);

        if (log is null)
        {
            var notFoundError = new Error("404", "Log is not found");
            
            return Result.Failure(notFoundError);
        }
        
        log.CheckStatus = request.CheckStatus;
        log.IsDeleted = request.IsDeleted;
        log.WorkoutTime = request.WorkoutTime;
        log.CreatedAt = request.CreatedAt;
        
        _checkLogRepo.Update(log);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        if (result) return Result.Success();
        
        var error = new Error("500", "Failed to update log");
        
        return Result.Failure(error);
        
    }
}