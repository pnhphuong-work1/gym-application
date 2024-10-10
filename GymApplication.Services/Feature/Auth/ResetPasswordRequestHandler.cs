using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class ResetPasswordRequestHandler : IRequestHandler<ResetPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordRequestHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
        {
            var error = new Error("404", "User not found");
            return Result.Failure(error);
        }
        
        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        
        if (result.Succeeded)
        {
            return Result.Success();
        }
        
        var errors = new Error("400", "Failed to reset password");
        return Result.Failure(errors);
    }
}