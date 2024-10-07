using GymApplication.Services.Extension;
using GymApplication.Shared.BusinessObject.Email;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace GymApplication.Services.Feature.Email;

public class SendEmailNotificationHandler : INotificationHandler<SendMailNotification>
{
    private readonly EmailOptions _emailOptions = new();

    public SendEmailNotificationHandler(IConfiguration configuration)
    {
        configuration.GetSection("EmailOptions").Bind(_emailOptions);
    }
    
    public async Task Handle(SendMailNotification notification, CancellationToken cancellationToken)
    {
        var email = new MimeMessage();
        
        email.From.Add(MailboxAddress.Parse(_emailOptions.From));
        email.To.Add(MailboxAddress.Parse(notification.To));
        email.Subject = notification.Subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = notification.Body
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailOptions.SmtpServer, 
            _emailOptions.Port, 
            true, 
            cancellationToken);
        await smtp.AuthenticateAsync(
            _emailOptions.UserName, 
            _emailOptions.Password, 
            cancellationToken);
        
        await smtp.SendAsync(email, cancellationToken);
        
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}