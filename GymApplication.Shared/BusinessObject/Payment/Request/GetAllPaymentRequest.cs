using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Payment.Request;

public sealed class GetAllPaymentRequest : IRequest<Result<PagedResult<PaymentReturnResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}