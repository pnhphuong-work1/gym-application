using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
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
    private readonly ICacheServices _cacheServices;
    
    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<UserResponse>> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        
        var cacheValue = await _cacheServices.GetAsync<UserResponse>(request.Id.ToString(), cancellationToken);
        if (cacheValue != null) return Result.Success(cacheValue);
        
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user != null)
        {
            var userResponse = _mapper.Map<UserResponse>(user);
            await _cacheServices.SetAsync(request.Id.ToString(), userResponse, TimeSpan.FromMinutes(5), cancellationToken);
            return Result.Success(userResponse);
        }
        
        Error error = new("404", "User not found");
        return Result.Failure<UserResponse>(error);
    }
}