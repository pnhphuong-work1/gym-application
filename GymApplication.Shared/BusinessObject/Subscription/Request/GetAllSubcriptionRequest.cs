using FluentValidation;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed class GetAllSubscriptionsRequest : IRequest<Result<PagedResult<SubscriptionResponse>>>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "email";
    public string SortOrder { get; set; } = "asc";
    public string? SortBy { get; set; } = "email";
}

public sealed class GetAllSubscriptionsRequestValidation : AbstractValidator<GetAllSubscriptionsRequest>
{
    public GetAllSubscriptionsRequestValidation()
    {
        RuleFor(s => s.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(s => s.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");
        
        RuleFor(s => s.SortBy)
            .Must(s => s is "name" or "price" or "totalWorkoutTime")
            .WithMessage("SortBy must be either name, price, totalWorkoutTime");

        RuleFor(s => s.SortOrder)
            .Must(s => s is "asc" or "desc")
            .WithMessage("SortOrder must be either asc or desc");
        
        RuleFor(s => s.SearchBy)
            .Must(s => s is "name" or "price" or "totalWorkoutTime")
            .WithMessage("SearchBy must be either name, price, totalWorkoutTime");
    }
}