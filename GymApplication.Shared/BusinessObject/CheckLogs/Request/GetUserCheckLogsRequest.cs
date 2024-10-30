using FluentValidation;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed class GetUserCheckLogsRequestValidation : AbstractValidator<GetUserCheckLogsRequest>
{
    public GetUserCheckLogsRequestValidation()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .WithMessage("UserId must not be empty");
        
        RuleFor(u => u.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(u => u.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");

        RuleFor(u => u.SortOrder)
            .Must(u => u is "asc" or "desc")
            .WithMessage("SortOrder must be either asc or desc");
        
        RuleFor(u => u.CheckStatus)
            .Must(u => u is "All" or "CheckIn" or "CheckOut")
            .WithMessage("CheckStatus must be either All, CheckIn, CheckOut");
        
        RuleFor(u => u.TimeFrame)
            .Must(u => u is "All" or "Today" or "Yesterday" or "ThisWeek" or "ThisMonth" or "LastMonth" or "90days")
            .WithMessage("TimeFrame must be either All, Today, Yesterday, ThisWeek, ThisMonth, LastMonth, 90days");
    }
}

public sealed class GetUserCheckLogsRequest : IRequest<Result<PagedResult<CheckLogsResponse>>>
{
    [FromRoute(Name = "userId")]
    public Guid UserId { get; set; }
    
    [FromQuery]
    public string? CheckStatus { get; set; } = "All";
    
    [FromQuery]
    public string? TimeFrame { get; set; } = "All";
    
    [FromQuery]
    public string SortOrder { get; set; } = "desc";
    
    [FromQuery]
    public int CurrentPage { get; set; } = 1;
    [FromQuery]
    public int PageSize { get; set; } = 10;
    
}