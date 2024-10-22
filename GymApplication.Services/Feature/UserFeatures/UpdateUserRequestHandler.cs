using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICacheServices _cacheServices; 

    public UpdateUserRequestHandler(UserManager<ApplicationUser> userManager, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _cacheServices = cacheServices;
    }

    public async Task<Result> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        
        if (user is null)
        {
            var notFoundError = new Error("User not found", "User not found");
            
            return Result.Failure(notFoundError);
        }
        
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            await _cacheServices.SetAsync(user.Id.ToString(), user, cancellationToken);
            await _cacheServices.RemoveByPrefixAsync("GetAll", cancellationToken);
            return Result.Success();
        }
        
        var error = new Error("Failed to update user", "Failed to update user");
            
        return Result.Failure(error);
    }
}