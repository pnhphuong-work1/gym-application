using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.Shared.BusinessObject.Payment.Request;

public sealed class GetPaymentByUserId : IRequest<Result<PagedResult<PaymentReturnResponse>>>
{
    [FromRoute(Name = "userId")]
    public Guid UserId { get; set; }
    
    [FromQuery]
    public int Page { get; set; }
    
    [FromQuery]
    public int PageSize { get; set; }
}