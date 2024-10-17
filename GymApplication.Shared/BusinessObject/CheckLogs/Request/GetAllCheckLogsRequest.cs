using FluentValidation;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed class GetAllCheckLogsRequestValidation : AbstractValidator<GetAllCheckLogsRequest>
{
    public GetAllCheckLogsRequestValidation()
    {
        RuleFor(u => u.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(u => u.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");

        RuleFor(u => u.SortBy)
            .Must(u => u is "fullName" or "createdAt" )
            .WithMessage("SortBy must be either fullName, createdAt");

        RuleFor(u => u.SortOrder)
            .Must(u => u is "asc" or "desc")
            .WithMessage("SortOrder must be either asc or desc");
        
        RuleFor(u => u.SearchBy)
            .Must(u => u is "fullName" or "subscriptionName")
            .WithMessage("SearchBy must be either fullName, subscriptionName");
        
        RuleFor(u => u.CheckStatus)
            .Must(u => u is "All" or "CheckIn" or "CheckOut")
            .WithMessage("CheckStatus must be either All, CheckIn, CheckOut");
        
        RuleFor(u => u.TimeFrame)
            .Must(u => u is "All" or "Today" or "Yesterday" or "ThisWeek" or "ThisMonth" or "90days")
            .WithMessage("TimeFrame must be either All, Today, Yesterday, ThisWeek, ThisMonth, 90days");
    }
}

public sealed class GetAllCheckLogsRequest : IRequest<Result<PagedResult<CheckLogsResponse>>>
{
    public string? CheckStatus { get; set; } = "All";
    public string? TimeFrame { get; set; } = "All";
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "fullName";
    public string SortOrder { get; set; } = "desc";
    public string? SortBy { get; set; } = "createdAt";
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}