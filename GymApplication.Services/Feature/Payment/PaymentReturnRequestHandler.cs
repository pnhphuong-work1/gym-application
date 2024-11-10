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
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOsServices _payOsServices;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public PaymentReturnRequestHandler(IPaymentLogRepository paymentLogRepository, IPayOsServices payOsServices, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, ISender sender, IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepository, IUserSubscriptionRepository userSubscriptionRepository)
    {
        _paymentLogRepository = paymentLogRepository;
        _payOsServices = payOsServices;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _sender = sender;
        _subscriptionRepository = subscriptionRepository;
        _userSubscriptionRepository = userSubscriptionRepository;
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
        
        var sub = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
        
        if (sub is null)
        {
            var error = new Error("404", "Subscription not found");
            return Result.Failure<PaymentReturnResponse>(error);
        }
        // cause The data is unreliable because the signature of the response does not match the signature of the data
        //var paymentDetail = await _payOsServices.GetPaymentDetail(request.OrderCode);

        var userSubs = (await _userSubscriptionRepository
            .GetByConditionsAsync(x => 
                x.UserId == paymentLog.UserId
                && x.SubscriptionId == request.SubscriptionId
                && x.SubscriptionEndDate >= DateTime.UtcNow)).FirstOrDefault();
        
        if (request.Cancel)
        {
            paymentLog.PaymentStatus = PaymentStatus.Cancel.ToString();
            _paymentLogRepository.Update(paymentLog);
        } 
        else if (request.Status == "PAID")
        {
            
            paymentLog.PaymentStatus = PaymentStatus.Success.ToString();
            _paymentLogRepository.Update(paymentLog);
            if (userSubs is not null)
            {
                userSubs.SubscriptionEndDate = userSubs.SubscriptionEndDate.AddDays(sub.TotalMonth * 30);
                _userSubscriptionRepository.Update(userSubs);
            }
            var createUserSubscriptionRequest = new CreateSubscriptionUserRequest()
            {
                UserId = paymentLog.UserId,
                SubscriptionId = request.SubscriptionId,
                PaymentId = paymentLog.Id,
                PaymentPrice = sub.Price,
                SubscriptionEndDate = paymentLog.PaymentDate.AddDays(sub.TotalMonth * 30)
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
            Amount = Convert.ToInt32(sub.Price),
            PaymentDate = paymentLog.PaymentDate,
            UserId = paymentLog.UserId,
            User = _mapper.Map<UserResponse>(paymentLog.User)
        };
        
        return Result.Success(res);
    }
}