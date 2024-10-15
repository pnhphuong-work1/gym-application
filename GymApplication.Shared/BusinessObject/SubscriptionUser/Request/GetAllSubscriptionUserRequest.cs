using FluentValidation;
using GymApplication.Shared.BusinessObject.SubscriptionUser.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed class GetAllSubscriptionUserRequestValidation : AbstractValidator<GetAllSubscriptionUserRequest>
{
    public GetAllSubscriptionUserRequestValidation()
    {
        RuleFor(s => s.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(s => s.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");
    }
}

public class GetAllSubscriptionUserRequest : IRequest<Result<PagedResult<SubscriptionUserResponse>>>
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "userName";
    public string SortOrder { get; set; } = "desc";
    public string? SortBy { get; set; } = "createdAt";
}