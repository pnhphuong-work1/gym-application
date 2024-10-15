using AutoMapper;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Payment.Request;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.Payment;

public sealed class GetPaymentByUserIdHandler : IRequestHandler<GetPaymentByUserId, Result<PagedResult<PaymentReturnResponse>>>
{
    private readonly IPaymentLogRepository _paymentLogRepository;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public GetPaymentByUserIdHandler(IPaymentLogRepository paymentLogRepository, IMapper mapper, ICacheServices cacheServices)
    {
        _paymentLogRepository = paymentLogRepository;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<PagedResult<PaymentReturnResponse>>> Handle(GetPaymentByUserId request, CancellationToken cancellationToken)
    {
        var key = $"paymentLogs_{request.UserId}_{request.Page}_{request.PageSize}";
        
        var cachedData = await _cacheServices.GetAsync<PagedResult<PaymentReturnResponse>>(key, cancellationToken);
        
        if (cachedData is not null)
        {
            return Result.Success(cachedData);
        }
        
        var paymentLogs = _paymentLogRepository.GetQueryable()
            .Include(x => x.User)
            .Include(x => x.UserSubscriptions)
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt);
        
        var response = paymentLogs.Select(p => _mapper.Map<PaymentReturnResponse>(p));
        
        var pagedResult = await PagedResult<PaymentReturnResponse>.CreateAsync(response, request.Page, request.PageSize);
        
        await _cacheServices.SetAsync(key, pagedResult, TimeSpan.FromMinutes(5), cancellationToken);
        
        return Result.Success(pagedResult);
    }
}