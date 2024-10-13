using GymApplication.Shared.BusinessObject.User.Response;

namespace GymApplication.Shared.BusinessObject.Payment.Response;

public sealed class PaymentReturnResponse
{
    public Guid Id { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public int Amount { get; set; }
    public Guid UserId { get; set; }
    
    public UserResponse? User { get; set; }
}