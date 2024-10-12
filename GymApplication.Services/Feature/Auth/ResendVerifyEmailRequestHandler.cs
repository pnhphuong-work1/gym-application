using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Email;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.Auth;

public sealed class ResendVerifyEmailRequestHandler : IRequestHandler<ResendVerifyEmailRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublisher _publisher;

    public ResendVerifyEmailRequestHandler(UserManager<ApplicationUser> userManager, IPublisher publisher)
    {
        _userManager = userManager;
        _publisher = publisher;
    }

    public async Task<Result> Handle(ResendVerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null || user.IsDeleted)
        {
            Error error = new("404", "User not found");
            return Result.Failure(error);
        }
        
        var verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // Encode the token and email before adding to the URL
        var encodedToken = Uri.EscapeDataString(verifyToken);
        var encodedEmail = Uri.EscapeDataString(request.Email);
        var verifyUrl = $"https://localhost:3000/verify-email?email={encodedEmail}&token={encodedToken}";
        var emailTemplate = Helper.GetEmailTemplate(user.FullName, verifyUrl);
        
        //Send email notification
        var emailNotification = new SendMailNotification
        {
            To = user.Email!,
            Subject = "Welcome to Gym Application",
            Body = emailTemplate
        };
        
        // use _ to discard the result, it will run in the background
        _ = _publisher.Publish(emailNotification, cancellationToken);
        
        return Result.Success();
    }
}