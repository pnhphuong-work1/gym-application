using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICacheServices _cacheServices;

    public DeleteUserRequestHandler(UserManager<ApplicationUser> userManager, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _cacheServices = cacheServices;
    }

    public async Task<Result> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        
        if (user is null)
        {
            var notFoundError = new Error("404", "User not found");
            
            return Result.Failure(notFoundError);
        }
        
        var result = await _userManager.DeleteAsync(user);
        
        if (result.Succeeded)
        {
            await _cacheServices.RemoveAsync(request.Id.ToString(), cancellationToken);
            await _cacheServices.RemoveByPrefixAsync("GetAll", cancellationToken);
            return Result.Success();
        }
        
        var error = new Error("500", "Failed to delete user");
        
        return Result.Failure(error);
    }
}