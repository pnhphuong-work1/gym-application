using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
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

        CreateMap<PaymentLog, PaymentReturnResponse>()
            .ForMember(x => x.User, opt => opt.MapFrom(x => x.User))
            .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.UserSubscriptions.Sum(u => u.PaymentPrice)));

        CreateMap<CheckLog, CheckLogsResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.SubscriptionName, opt => opt.MapFrom(src => src.UserSubscription.Subscription.Name))
            .ReverseMap();

        CreateMap<PagedResult<CheckLog>, PagedResult<CheckLogsResponse>>()
            .ReverseMap();
        
        CreateMap<Subscription, SubscriptionResponse>()
            .ForMember(s => s.Group,
                opt => opt.MapFrom(s => s.DayGroup.Group))
            .ReverseMap();
        
        CreateMap<PagedResult<Subscription>, PagedResult<SubscriptionResponse>>()
            .ReverseMap();

        CreateMap<DayGroup, DayGroupResponse>()
            .ReverseMap();

        CreateMap<PagedResult<DayGroup>, PagedResult<DayGroupResponse>>()
            .ReverseMap();
        
        CreateMap<UserSubscription, SubscriptionUserResponse>()
            .ForMember(su => su.SubscriptionStartDate,
                opt => opt.MapFrom(su => su.Payment.PaymentDate))
            .ForMember(su => su.Group,
                opt => opt.MapFrom(su => su.Subscription.DayGroup.Group))
            .ForMember(su => su.TotalWorkoutTime,
                opt => opt.MapFrom(su => su.Subscription.TotalWorkoutTime))
            .ForMember(su => su.Name,
                opt => opt.MapFrom(su => su.Subscription.Name))
            .ReverseMap();
        CreateMap<PagedResult<UserSubscription>, PagedResult<SubscriptionUserResponse>>()
            .ReverseMap();
    }
}