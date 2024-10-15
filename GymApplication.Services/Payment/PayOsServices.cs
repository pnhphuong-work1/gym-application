using GymApplication.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace GymApplication.Services.Payment;

public class PayOsServices : IPayOsServices
{
    private readonly PayOS _payOs;

    public PayOsServices(IConfiguration configuration)
    {
        var clientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("ClientId is required");
        var apiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("ApiKey is required");
        var checkSumKey = configuration["PayOS:CheckSumKey"] ?? throw new ArgumentNullException("CheckSumKey is required");
        
        _payOs = new PayOS(clientId, apiKey, checkSumKey);
    }


    public async Task<CreatePaymentResult> CreatePayment(int totalAmount, List<ItemData> paymentItems, 
        string returnUrl, string cancelUrl, string description = "", long orderCode = 0)
    {
        var paymentData = new PaymentData(
            orderCode,
            totalAmount,
            description,
            paymentItems,
            returnUrl: returnUrl,
            cancelUrl: cancelUrl
        );
        
        return await _payOs.createPaymentLink(paymentData);
    }

    public async Task<PaymentLinkInformation> GetPaymentDetail(long payOsOrderId)
    {
        var paymentDetail = await _payOs.getPaymentLinkInformation(payOsOrderId);

        return paymentDetail;
    }
}