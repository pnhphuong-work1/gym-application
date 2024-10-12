using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.Revenue.Request;
using GymApplication.Shared.BusinessObject.Revenue.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.Revenue;

public class GetRevenueRequestHandler : IRequestHandler<GetRevenueRequest, Result<RevenueResponse>>
{
    private readonly IPaymentLogRepository _paymentLogRepository;

    public GetRevenueRequestHandler(IPaymentLogRepository paymentLogRepository)
    {
        _paymentLogRepository = paymentLogRepository;
    }

    public async Task<Result<RevenueResponse>> Handle(GetRevenueRequest request, CancellationToken cancellationToken)
    {
        var paymentLogs = await _paymentLogRepository.GetByConditionsAsync(x => x.CreatedAt.Month == request.Month && x.CreatedAt.Year == request.Year);
}