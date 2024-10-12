using System.Linq.Expressions;
using GymApplication.Repository.Entities;
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

    public GetAllCustomerRequestHandler(UserManager<ApplicationUser> userManager, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _cacheServices = cacheServices;
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
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Payments)
            .ThenInclude(x => x.UserSubscriptions)
            .Include(u => u.CheckLogs)
            .AsSplitQuery()
            .Where(u => u.IsDeleted == false);
        
        Expression<Func<ApplicationUser, object>> sortBy = request.SortBy switch
        {
            "fullName" => u => u.FullName!,
            "email" => u => u.Email!,
            "phoneNumber" => u => u.PhoneNumber!,
            "dateOfBirth" => u => u.DateOfBirth,
            _ => u => u.Email!
        };
        
        users = request.SortOrder switch
        {
            "asc" => users.OrderBy(sortBy),
            "desc" => users.OrderByDescending(sortBy),
            _ => users.OrderBy(sortBy)
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
            users = users.Where(searchBy);
        }

        var res = users.Select(u => new CustomerResponse()
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
        
        await _cacheServices.SetAsync(redisKey, pagedResult, cancellationToken);
        
        return Result.Success(pagedResult);
    }
}