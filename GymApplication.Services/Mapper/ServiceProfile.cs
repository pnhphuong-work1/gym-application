using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.User.Response;

namespace GymApplication.Services.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<ApplicationUser, UserResponse>()
            .ReverseMap();
    }
}