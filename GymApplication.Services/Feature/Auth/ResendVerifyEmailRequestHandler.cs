using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Auth.Request;
using GymApplication.Shared.BusinessObject.Email;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace GymApplication.Services.Feature.Auth;

public sealed class ResendVerifyEmailRequestHandler : IRequestHandler<ResendVerifyEmailRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public ResendVerifyEmailRequestHandler(UserManager<ApplicationUser> userManager, IPublisher publisher, IConfiguration configuration)
    {
        _userManager = userManager;
        _publisher = publisher;
        _configuration = configuration;
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
        var baseUrl = _configuration["FEApp:BaseUrl"];
        var verifyUrl = $"{baseUrl}/verify-email?email={encodedEmail}&token={encodedToken}";
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