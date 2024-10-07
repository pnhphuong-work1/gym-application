using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class VerifyEmailRequestHandler : IRequestHandler<VerifyEmailRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public VerifyEmailRequestHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            Error error = new("404", "User not found");
            return Result.Failure(error);
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            Error error = new("400", "Email verification failed");
            return Result.Failure(error);
        }

        return Result.Success();
    }
}