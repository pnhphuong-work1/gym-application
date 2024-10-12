using GymApplication.Shared.Emuns;

namespace GymApplication.Shared.BusinessObject.Payment.Response;

public sealed class PaymentResponse
{
    public Guid Id { get; set; }
    public string Bin { get; set; } = string.Empty;
    public int Amount { get; set; }
    public long PayOsOrderId { get; set; }
    public string? Description { get; set; }
    public string PaymentLinkId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string QrCode { get; set; } = string.Empty;
}