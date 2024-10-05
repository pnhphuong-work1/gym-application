using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class ChangePasswordRequestHandler : IRequestHandler<ChangePasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordRequestHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
        {
            var error = new Error("404", "User not found");
            return Result.Failure(error);
        }
        
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded) return Result.Success();
        {
            var error = new Error("400", "Password change failed");
            return Result.Failure(error);
        }

    }
}