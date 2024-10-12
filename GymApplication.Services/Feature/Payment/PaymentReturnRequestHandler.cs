using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Payment.Request;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.Payment;

public sealed class PaymentReturnRequestHandler : IRequestHandler<PaymentReturnRequest, Result<PaymentReturnResponse>>
{
    private readonly IPaymentLogRepository _paymentLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOsServices _payOsServices;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public PaymentReturnRequestHandler(IPaymentLogRepository paymentLogRepository, IPayOsServices payOsServices, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _paymentLogRepository = paymentLogRepository;
        _payOsServices = payOsServices;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<PaymentReturnResponse>> Handle(PaymentReturnRequest request, CancellationToken cancellationToken)
    {
        var paymentLog = await _paymentLogRepository.GetQueryable()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.PayOsOrderId == request.OrderCode, cancellationToken);


        if (paymentLog is null)
        {
            var error = new Error("404", "Payment not found");
            return Result.Failure<PaymentReturnResponse>(error);
        }
        
        if (request.Cancel)
        {
            paymentLog.PaymentStatus = PaymentStatus.Cancel.ToString();
            _paymentLogRepository.Update(paymentLog);
        } 
        else
        {
            paymentLog.PaymentStatus = PaymentStatus.Success.ToString();
            _paymentLogRepository.Update(paymentLog);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var paymentDetail = await _payOsServices.GetPaymentDetail(request.OrderCode);

        var res = new PaymentReturnResponse()
        {
            Id = paymentLog.Id,
            PaymentStatus = paymentLog.PaymentStatus,
            Amount = paymentDetail.amount,
            PaymentDate = paymentLog.PaymentDate,
            UserId = paymentLog.UserId,
            User = _mapper.Map<UserResponse>(paymentLog.User)
        };
        
        return Result.Success(res);
    }
}