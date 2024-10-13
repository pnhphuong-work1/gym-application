using FluentValidation;
using GymApplication.Shared.BusinessObject.Subscription.Respone;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed class GetAllSubscriptionsRequest : IRequest<Result<PagedResult<SubscriptionResponse>>>
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "name";
    public string SortOrder { get; set; } = "desc";
    public string? SortBy { get; set; } = "createdAt";
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
    }
}