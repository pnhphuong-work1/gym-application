using MediatR;

namespace GymApplication.Shared.BusinessObject.Email;

public class SendMailNotification : INotification
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}