using System.Linq.Expressions;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroup.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public sealed class UpdateDayGroupHandler : IRequestHandler<UpdateDayGroupRequest, Result>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDayGroupHandler(IRepoBase<DayGroup, Guid> dayGroupRepository,
        ICacheServices cacheServices, IUnitOfWork unitOfWork)
    {
        _dayGroupRepository = dayGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateDayGroupRequest request, CancellationToken cancellationToken)
    {
        var dayGroup = await _dayGroupRepository.GetByIdAsync(request.Id);

        if (dayGroup is null)
        {
            var notFoundError = new Error("DayGroup not found", "DayGroup not found");

            return Result.Failure(notFoundError);
        }

        dayGroup.Group = request.Group;
        dayGroup.UpdatedAt = DateTime.Now;

        _dayGroupRepository.Update(dayGroup);
        
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result) return Result.Success();

        var error = new Error("500", "Update Day Group Failed");
        return Result.Failure<DayGroupResponse>(error);
    }
}