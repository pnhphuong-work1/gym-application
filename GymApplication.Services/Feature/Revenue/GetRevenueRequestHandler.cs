using System.Globalization;
using System.Linq.Expressions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Shared.BusinessObject.Revenue.Request;
using GymApplication.Shared.BusinessObject.Revenue.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
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
        Expression<Func<PaymentLog, object>>[] includes =
        [
            x => x.UserSubscriptions
        ];

        var paymentLogs = await _paymentLogRepository
            .GetByConditionsAsync(x =>
                    x.CreatedAt.Month == request.Month
                    && x.CreatedAt.Year == request.Year
                    && x.PaymentStatus == PaymentStatus.Success.ToString(),
                includes
            );

        var totalMember = paymentLogs.Select(x => x.UserId).Distinct().Count();
        var totalRevenue = paymentLogs.Select(x => x.UserSubscriptions.Select(us => us.PaymentPrice).Sum()).Sum();
        var calendar = CultureInfo.InvariantCulture.Calendar;
        var weeklyRevenue = paymentLogs
            .GroupBy(x => GetWeekOfMonth(x.CreatedAt, calendar))
            .Select(x => new WeeklyRevenueResponse
            {
                Week = x.Key.ToString(),
                Revenue = x.Select(y => y.UserSubscriptions.Select(us => us.PaymentPrice).Sum()).Sum()
            })
            .ToList();
        
        var response = new RevenueResponse
        {
            TotalMember = totalMember,
            TotalRevenue = totalRevenue,
            WeeklyRevenue = weeklyRevenue
        };
        
        return Result.Success(response);
    }
    
    private static int GetWeekOfMonth(DateTime date, Calendar calendar)
    {
        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

        // Find the first Monday of the month
        var firstMonday = firstDayOfMonth;
        while (firstMonday.DayOfWeek != DayOfWeek.Monday)
        {
            firstMonday = firstMonday.AddDays(1);
        }

        // Calculate the number of full weeks between the first Monday and the given date
        var daysDifference = (date - firstMonday).Days;
        return (daysDifference / 7) + 1;
    }
}