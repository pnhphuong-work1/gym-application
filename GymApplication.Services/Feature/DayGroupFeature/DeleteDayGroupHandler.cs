using System.Linq.Expressions;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public sealed class DeleteDayGroupHandler : IRequestHandler<DeleteDayGroupRequest, Result>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDayGroupHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IUnitOfWork unitOfWork)
    {
        _dayGroupRepository = dayGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteDayGroupRequest request, CancellationToken cancellationToken)
    {
            var dayGroup = await _dayGroupRepository.GetByIdAsync(request.Id, null);

            if (dayGroup is null)
            {
                var notFoundError = new Error("404", "DayGroup not found");

                return Result.Failure(notFoundError);
            }

            _dayGroupRepository.Delete(dayGroup);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            if (result) return Result.Success();

            var error = new Error("500", "Delete DayGroup Faied");
            return Result.Failure<DayGroupResponse>(error);
    }
}