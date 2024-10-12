using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;

namespace GymApplication.Services.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<ApplicationUser, UserResponse>()
            .ReverseMap();

        CreateMap<PagedResult<ApplicationUser>, PagedResult<UserResponse>>()
            .ReverseMap();

        CreateMap<CheckLog, CheckLogsResponse>()
            .ReverseMap();

        CreateMap<PagedResult<CheckLog>, PagedResult<CheckLogsResponse>>()
            .ReverseMap();
        
        CreateMap<Subscription, SubscriptionResponse>()
            .ReverseMap();
        
        CreateMap<PagedResult<Subscription>, PagedResult<SubscriptionResponse>>()
            .ReverseMap();

        CreateMap<DayGroup, DayGroupResponse>()
            .ReverseMap();

        CreateMap<PagedResult<DayGroup>, PagedResult<DayGroupResponse>>()
            .ReverseMap();
    }
}