using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Payment.Request;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
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
    private readonly ISender _sender;

    public PaymentReturnRequestHandler(IPaymentLogRepository paymentLogRepository, IPayOsServices payOsServices, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, ISender sender)
    {
        _paymentLogRepository = paymentLogRepository;
        _payOsServices = payOsServices;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _sender = sender;
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
        var paymentDetail = await _payOsServices.GetPaymentDetail(request.OrderCode);
        
        if (request.Cancel)
        {
            paymentLog.PaymentStatus = PaymentStatus.Cancel.ToString();
            _paymentLogRepository.Update(paymentLog);
        } 
        else if (request.Status == "PAID")
        {
            paymentLog.PaymentStatus = PaymentStatus.Success.ToString();
            _paymentLogRepository.Update(paymentLog);
            var createUserSubscriptionRequest = new CreateSubscriptionUserRequest()
            {
                UserId = paymentLog.UserId,
                SubscriptionId = request.SubscriptionId,
                PaymentId = paymentLog.Id,
                PaymentPrice = paymentDetail.amount,
                SubscriptionEndDate = paymentLog.PaymentDate.AddDays(30)
            };
            
            var result = await _sender.Send(createUserSubscriptionRequest, cancellationToken);
            
            if (result.IsFailure)
            {
                return Result.Failure<PaymentReturnResponse>(result.Error);
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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