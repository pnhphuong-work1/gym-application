using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;
using UUIDNext;

namespace GymApplication.Services.Feature.DayGroupFeature;

public class CreateDayGroupHandler : IRequestHandler<CreateDayGroupRequest, Result<DayGroupResponse>>
{
    private readonly IRepoBase<DayGroup, Guid> _dayGroupRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDayGroupHandler(IRepoBase<DayGroup, Guid> dayGroupRepository, IMapper mapper,
        ICacheServices cacheServices, IUnitOfWork unitOfWork)
    {
        _dayGroupRepository = dayGroupRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
        _unitOfWork = unitOfWork;
    }

    public Task<Result<DayGroupResponse>> Handle(CreateDayGroupRequest request,
        CancellationToken cancellationToken)
    {
        var dayGroup = new DayGroup()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Group = request.Group,
            CreatedAt = DateTime.UtcNow
        };
        _dayGroupRepository.Add(dayGroup);
        _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<DayGroupResponse>(dayGroup);

        _cacheServices.SetAsync(dayGroup.Id.ToString(), response, TimeSpan.FromMinutes(5), cancellationToken);

        return Task.FromResult(Result.Success(response));
    }
}