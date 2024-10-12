using GymApplication.Shared.BusinessObject.Payment.Response;
using Net.payOS.Types;

namespace GymApplication.Services.Abstractions;

public interface IPayOsServices
{
    Task<CreatePaymentResult> CreatePayment(
        int totalAmount,
        List<ItemData> paymentItems,
        string description = "",
        long orderCode = 0
    );
    
    Task<PaymentLinkInformation> GetPaymentDetail(long payOsOrderId);
}