using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.BusinessObject.Payment.Response;
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
            .ReverseMap();

        CreateMap<PagedResult<CheckLog>, PagedResult<CheckLogsResponse>>()
            .ReverseMap();
    }
}