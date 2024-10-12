using GymApplication.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace GymApplication.Services.Payment;

public class PayOsServices : IPayOsServices
{
    private readonly IConfiguration _configuration;
    private readonly PayOS _payOs;

    public PayOsServices(IConfiguration configuration)
    {
        _configuration = configuration;
        var clientId = _configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("ClientId is required");
        var apiKey = _configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("ApiKey is required");
        var checkSumKey = _configuration["PayOS:CheckSumKey"] ?? throw new ArgumentNullException("CheckSumKey is required");
        
        _payOs = new PayOS(clientId, apiKey, checkSumKey);
    }


    public async Task<CreatePaymentResult> CreatePayment(int totalAmount, List<ItemData> paymentItems, string description = "",
        long orderCode = 0)
    {
        var baseReturnUrl = _configuration["PayOS:BaseReturnUrl"] ?? throw new ArgumentNullException("ReturnUrl is required");
        var paymentData = new PaymentData(
            orderCode,
            totalAmount,
            description,
            paymentItems,
            baseReturnUrl,
            baseReturnUrl
        );
        
        return await _payOs.createPaymentLink(paymentData);
    }

    public async Task<PaymentLinkInformation> GetPaymentDetail(long payOsOrderId)
    {
        var paymentDetail = await _payOs.getPaymentLinkInformation(payOsOrderId);

        return paymentDetail;
    }
}