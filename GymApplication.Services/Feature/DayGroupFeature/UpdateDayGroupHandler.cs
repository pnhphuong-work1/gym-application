using System.Linq.Expressions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroup.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Services.Feature.DayGroupFeature;

public sealed class UpdateDayGroupHandler : IRequestHandler<UpdateDayGroupRequest, Result>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly ICacheServices _cacheServices; 

    public UpdateDayGroupHandler(IRepoBase<DayGroup, Guid> dayGroupRepoBase, ICacheServices cacheServices)
    {
        _dayGroupRepository = dayGroupRepoBase;
        _cacheServices = cacheServices;
    }

    public async Task<Result> Handle(UpdateDayGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dayGroup = await _dayGroupRepository.GetByIdAsync(request.Id, new Expression<Func<DayGroup, object>>[0]);

            if (dayGroup is null)
            {
                var notFoundError = new Error("DayGroup not found", "DayGroup not found");
            
                return Result.Failure(notFoundError);
            }

            dayGroup.Group = request.Group;
            dayGroup.UpdatedAt = DateTime.Now;

            _dayGroupRepository.Update(dayGroup);

            await _cacheServices.SetAsync(dayGroup.Id.ToString(), dayGroup, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var error = new Error("Failed to update DayGroup", ex.Message);
            return Result.Failure(error);
        }
    }
}