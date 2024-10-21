using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class GetCustomerByIdHandler : IRequestHandler<GetCustomerById, Result<CustomerResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetCustomerByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<CustomerResponse>> Handle(GetCustomerById request, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .Users
            .Include(x => x.UserSubscriptions)
            .ThenInclude(x => x.Subscription)
            .Include(x => x.CheckLogs)
            .Include(u => u.Payments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (user != null)
        {
            var userResponse = new CustomerResponse
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                DateOfBirth = user.DateOfBirth,
                UserName = user.UserName ?? "",
                TotalSpentTime = user.CheckLogs.Where(x => x.CheckStatus == LogsStatus.CheckOut.ToString())
                    .Sum(x => x.WorkoutTime?.Hour ?? 0),
                TotalPayment = user.UserSubscriptions.Sum(us => us.PaymentPrice),
                //TotalPayment = user.Payments.Select(x => x.UserSubscriptions.Sum(us => us.PaymentPrice)).Sum(),
                Subscriptions = user.UserSubscriptions.Select(us => _mapper.Map<SubscriptionResponse>(us.Subscription)).ToList()
            };
            return Result.Success(userResponse);
        }

        Error error = new("404", "User not found");
        return Result.Failure<CustomerResponse>(error);
    }
}