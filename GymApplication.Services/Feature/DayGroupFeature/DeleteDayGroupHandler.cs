using System.Linq.Expressions;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public sealed class DeleteDayGroupHandler : IRequestHandler<DeleteDayGroupRequest, Result>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly ICacheServices _cacheServices;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDayGroupHandler(IRepoBase<DayGroup, Guid> dayGroupRepository,
        ICacheServices cacheServices, IUnitOfWork unitOfWork)
    {
        _dayGroupRepository = dayGroupRepository;
        _cacheServices = cacheServices;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteDayGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dayGroup =
                await _dayGroupRepository.GetByIdAsync(request.Id, new Expression<Func<DayGroup, object>>[0]);

            if (dayGroup is null)
            {
                var notFoundError = new Error("404", "DayGroup not found");

                return Result.Failure(notFoundError);
            }

            _dayGroupRepository.Delete(dayGroup);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheServices.RemoveAsync(request.Id.ToString(), cancellationToken);
            return Result.Success();
        }
        catch(Exception ex)
        {
            var error = new Error("500", "Failed to delete DayGroup");
        
            return Result.Failure(error);
        }
    }
}