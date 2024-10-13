using System.Linq.Expressions;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class GetAllCustomerRequestHandler : IRequestHandler<GetAllCustomerRequest, Result<PagedResult<CustomerResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICacheServices _cacheServices;
    private readonly IRepoBase<PaymentLog, Guid> _paymentLogRepo;
    private readonly IRepoBase<CheckLog, Guid> _checkLogRepo;
    public GetAllCustomerRequestHandler(UserManager<ApplicationUser> userManager, ICacheServices cacheServices, IRepoBase<CheckLog, Guid> checkLogRepo, IRepoBase<PaymentLog, Guid> paymentLogRepo)
    {
        _userManager = userManager;
        _cacheServices = cacheServices;
        _checkLogRepo = checkLogRepo;
        _paymentLogRepo = paymentLogRepo;
    }

    public async Task<Result<PagedResult<CustomerResponse>>> Handle(GetAllCustomerRequest request, CancellationToken cancellationToken)
    {
        var redisKey = $"GetAllCustomer:{request.CurrentPage}:{request.PageSize}:{request.SortBy}:{request.SortOrder}:{request.Search}:{request.SearchBy}";

        var cache = await _cacheServices.GetAsync<PagedResult<CustomerResponse>>(redisKey, cancellationToken);
        
        if (cache is not null)
        {
            return Result.Success(cache);
        }

        var users = (await _userManager.GetUsersInRoleAsync(Role.User.ToString()))
            .Where(u => u.IsDeleted == false)
            .ToList();
        
        var userIds = users.Select(u => u.Id).ToList();
        
        var paymentLog = await _paymentLogRepo
            .GetByConditionsAsync(l => userIds.Contains(l.UserId), [l => l.UserSubscriptions]);
        
        var checkLog = await _checkLogRepo
            .GetByConditionsAsync(l => userIds.Contains(l.UserId));

        var usersWithPayment = users.Select(u => new ApplicationUser()
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            UserName = u.UserName,
            Payments = paymentLog.Where(l => l.UserId == u.Id).ToList(),
            CheckLogs = checkLog.Where(l => l.UserId == u.Id).ToList()
        }).AsQueryable();
        
        Expression<Func<ApplicationUser, object>> sortBy = request.SortBy switch
        {
            "fullName" => u => u.FullName!,
            "email" => u => u.Email!,
            "phoneNumber" => u => u.PhoneNumber!,
            "dateOfBirth" => u => u.DateOfBirth,
            _ => u => u.Email!
        };
        
        usersWithPayment = request.SortOrder switch
        {
            "asc" => usersWithPayment.OrderBy(sortBy),
            "desc" => usersWithPayment.OrderByDescending(sortBy),
            _ => usersWithPayment.OrderBy(sortBy)
        };
        
        Expression<Func<ApplicationUser, bool>> searchBy = request.SearchBy switch
        {
            "fullName" => u => u.FullName!.Contains(request.Search!),
            "email" => u => u.Email!.Contains(request.Search!),
            "phoneNumber" => u => u.PhoneNumber!.Contains(request.Search!),
            "dateOfBirth" => u => u.DateOfBirth.ToString().Contains(request.Search!),
            _ => u => u.Email!.Contains(request.Search!)
        };
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            usersWithPayment = usersWithPayment.Where(searchBy);
        }

        var res = usersWithPayment.Select(u => new CustomerResponse()
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            UserName = u.UserName,
            TotalPayment = u.Payments.Select(x => x.UserSubscriptions.Sum(x => x.PaymentPrice)).Sum(),
            TotalSendTime = u.CheckLogs.Where(x => x.CheckStatus == LogsStatus.CheckOut.ToString())
                .Sum(x => x.WorkoutTime!.Value.Hour),
        }).ToList();
        
        var pagedResult = PagedResult<CustomerResponse>.Create(res, request.CurrentPage, request.PageSize);
        
        await _cacheServices.SetAsync(redisKey, pagedResult, TimeSpan.FromMinutes(5), cancellationToken);
        
        return Result.Success(pagedResult);
    }
}