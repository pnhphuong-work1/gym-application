using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Payment.Request;

public sealed class PaymentReturnRequest : IRequest<Result<PaymentReturnResponse>>
{
    public string Code { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public bool Cancel { get; set; }
    public string Status { get; set; } = string.Empty;
    public long OrderCode { get; set; }
}