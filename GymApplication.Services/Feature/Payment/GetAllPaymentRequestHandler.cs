using AutoMapper;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Payment.Request;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.Payment;

public class GetAllPaymentRequestHandler : IRequestHandler<GetAllPaymentRequest, Result<PagedResult<PaymentReturnResponse>>>
{
    private readonly IPaymentLogRepository _paymentLogRepository;
    private readonly ICacheServices _cacheServices;
    private readonly IMapper _mapper;

    public GetAllPaymentRequestHandler(IPaymentLogRepository paymentLogRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _paymentLogRepository = paymentLogRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<PagedResult<PaymentReturnResponse>>> Handle(GetAllPaymentRequest request, CancellationToken cancellationToken)
    {
        var key = $"paymentLogs_{request.Page}_{request.PageSize}";
        
        var cachedData = await _cacheServices.GetAsync<PagedResult<PaymentReturnResponse>>(key, cancellationToken);
        
        if (cachedData is not null)
        {
            return Result.Success(cachedData);
        }
        
        var paymentLogs = _paymentLogRepository.GetQueryable()
            .Include(x => x.User)
            .Include(x => x.UserSubscriptions)
            .Where(x => x.PaymentStatus == PaymentStatus.Success.ToString())
            .OrderByDescending(x => x.CreatedAt);

        var response = paymentLogs
            .Select(x => _mapper.Map<PaymentReturnResponse>(x));
        
        var pagedResult = await PagedResult<PaymentReturnResponse>.CreateAsync(response, request.Page, request.PageSize);
        
        await _cacheServices.SetAsync(key, pagedResult, TimeSpan.FromMinutes(5), cancellationToken);
        
        return Result.Success(pagedResult);
    }
}