using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.Payment.Request;
using GymApplication.Shared.BusinessObject.Payment.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Net.payOS.Types;
using UUIDNext;

namespace GymApplication.Services.Feature.Payment;

public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentRequest, Result<PaymentResponse>> 
{
    private readonly IPaymentLogRepository _paymentLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOsServices _payOsServices;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public CreatePaymentHandler(IPaymentLogRepository paymentLogRepository, IPayOsServices payOsServices, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _paymentLogRepository = paymentLogRepository;
        _payOsServices = payOsServices;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaymentResponse>> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            var error = new Error("404", "User not found");
            return Result.Failure<PaymentResponse>(error);
        }
        // var sub = await _subscriptionRepository.GetEntityByConditionAsync(s => s.Id == request.SubscriptionId, false, cancellationToken);
        var sub = new Repository.Entities.Subscription()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            Price = 1000,
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        var items = new List<ItemData>
        {
            new ItemData
            (
                 sub.Name, 
                 Convert.ToInt32(sub.Price),
                 1
            )
        };
        
        var orderCode = _paymentLogRepository.GetQueryable()
            .Select(pl => pl.PayOsOrderId)
            .Max() + 1;
        
        var paymentLog = new PaymentLog
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            UserId = request.UserId,
            PaymentStatus = PaymentStatus.Pending.ToString(),
            PayOsOrderId = orderCode,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        _paymentLogRepository.Add(paymentLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var paymentLink = await _payOsServices.CreatePayment(
                Convert.ToInt32(sub.Price),
                items,
                "Mua gói " + sub.Name,
                orderCode
            );

        var response = new PaymentResponse
        {
            Id = paymentLog.Id,
            PayOsOrderId = orderCode,
            Amount = paymentLink.amount,
            Bin = paymentLink.bin,
            CheckoutUrl = paymentLink.checkoutUrl,
            Description = paymentLink.description,
            PaymentLinkId = paymentLink.paymentLinkId,
            QrCode = paymentLink.qrCode,
            Status = paymentLink.status
        };
        
        return Result.Success(response);
    }
}