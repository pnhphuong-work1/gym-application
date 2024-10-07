using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Email;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class ForgotPasswordRequestHandler : IRequestHandler<ForgotPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublisher _publisher;

    public ForgotPasswordRequestHandler(UserManager<ApplicationUser> userManager, IPublisher publisher)
    {
        _userManager = userManager;
        _publisher = publisher;
    }

    public async Task<Result> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null || user.IsDeleted)
        {
            Error error = new("404", "User not found");
            return Result.Failure(error);
        }
        
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Encode the token and email before adding to the URL
        var encodedToken = Uri.EscapeDataString(resetToken);
        var encodedEmail = Uri.EscapeDataString(request.Email);
        
        var resetUrl = $"https://localhost:7294/api/v2024-09-29/user/reset-password?email={encodedEmail}&token={encodedToken}";
        var emailTemplate = Helper.GetForgotPasswordEmailTemplate(user.FullName, resetUrl);
        
        //Send email notification
        var emailNotification = new SendMailNotification
        {
            To = user.Email!,
            Subject = "Reset Password",
            Body = emailTemplate
        };
        
        // use _ to discard the result, it will run in the background
        _ = _publisher.Publish(emailNotification, cancellationToken);
        
        return Result.Success();
    }
}