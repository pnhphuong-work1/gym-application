using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Request;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UUIDNext;

namespace GymApplication.Services.Feature.SubscriptionUser;

public sealed class CreateSubscriptionUserHandler : IRequestHandler<CreateSubscriptionUserRequest, Result<SubscriptionUserResponse>>
{
    private readonly IRepoBase<UserSubscription, Guid> _subscriptionUserRepo;
    private readonly IRepoBase<Repository.Entities.Subscription, Guid> _subscriptionRepo;
    private readonly IPaymentLogRepository _paymentLogRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSubscriptionUserHandler(IRepoBase<UserSubscription, Guid> subscriptionUserRepo, IRepoBase<Repository.Entities.Subscription, Guid> subscriptionRepo, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IPaymentLogRepository paymentLogRepo, IMapper mapper)
    {
        _subscriptionUserRepo = subscriptionUserRepo;
        _subscriptionRepo = subscriptionRepo;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _paymentLogRepo = paymentLogRepo;
        _mapper = mapper;
    }


    public async Task<Result<SubscriptionUserResponse>> Handle(CreateSubscriptionUserRequest request, CancellationToken cancellationToken)
    {
        //Check if user exist
        var existedUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (existedUser is null)
        {
            Error error = new("404", "User not found");
            return Result.Failure<SubscriptionUserResponse>(error);
        }
        //Check existed subscription
        var existedSubscription = await _subscriptionRepo.GetByIdAsync(request.SubscriptionId);
        if (existedSubscription is null)
        {
            Error error = new("404", "Subscription type not found");
            return Result.Failure<SubscriptionUserResponse>(error);
        }
        //Check if user is already got the same subscription
        var existedUserSubs = await _subscriptionUserRepo
            .GetByConditionsAsync(x => 
                x.UserId == request.UserId
                & x.SubscriptionId == request.SubscriptionId);
        if (existedUserSubs.Count != 0)
        {
            Error error = new("404", "User already have this subscription.");
            return Result.Failure<SubscriptionUserResponse>(error);
        }
        //Check existed payment
        var existedPayment = await _paymentLogRepo.GetByIdAsync(request.PaymentId);
        if (existedPayment is null)
        {
            Error error = new("404", "Payment not found");
            return Result.Failure<SubscriptionUserResponse>(error);
        }
        //Create new subscriptionUser to add to DB
        var newSubscriptionUser = new UserSubscription()
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            UserId = request.UserId,
            PaymentId = request.PaymentId,
            SubscriptionId = request.SubscriptionId,
            PaymentPrice = request.PaymentPrice,
            WorkoutSteak = request.WorkoutSteak ?? 0,
            LongestWorkoutSteak = request.LongestWorkoutSteak ?? 0,
            LastWorkoutDate = request.LastWorkoutDate ?? DateTime.UtcNow.AddHours(7),
            SubscriptionEndDate = request.SubscriptionEndDate.Date.ToUniversalTime(),
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };
        Console.WriteLine(DateTime.UtcNow.Date.AddMonths(1).AddDays(-1));
        //Add to DB
        _subscriptionUserRepo.Add(newSubscriptionUser);
        var isCreated = await _unitOfWork.SaveChangesAsync(cancellationToken);
        //Handle failure
        if (!isCreated)
        {
            Error error = new("500", "Error occur while create subscription for user");
            return Result.Failure<SubscriptionUserResponse>(error);
        }

        // var response = new SubscriptionUserResponse()
        // {
        //     Id = newSubscriptionUser.Id,
        //     UserId = newSubscriptionUser.UserId,
        //     PaymentId = newSubscriptionUser.PaymentId,
        //     SubscriptionId = newSubscriptionUser.SubscriptionId,
        //     PaymentPrice = newSubscriptionUser.PaymentPrice,
        //     WorkoutSteak = newSubscriptionUser.WorkoutSteak,
        //     LongestWorkoutSteak = newSubscriptionUser.LongestWorkoutSteak,
        //     LastWorkoutDate = newSubscriptionUser.LastWorkoutDate,
        //     SubscriptionEndDate = newSubscriptionUser.SubscriptionEndDate
        // };
        
        var response = _mapper.Map<SubscriptionUserResponse>(newSubscriptionUser);
        return Result.Success(response);
    }
}
