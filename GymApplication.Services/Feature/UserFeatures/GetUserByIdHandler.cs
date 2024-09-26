using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserById, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user != null) return Result.Success(_mapper.Map<UserResponse>(user));
        
        Error error = new("404", "User not found"); 
        return Result.Failure<UserResponse>(error);
    }
}