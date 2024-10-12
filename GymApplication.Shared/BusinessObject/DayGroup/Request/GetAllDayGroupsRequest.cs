using FluentValidation;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;


namespace GymApplication.Shared.BusinessObject.DayGroups.Request;

public sealed class GetAllDayGroupsRequest : IRequest<Result<PagedResult<DayGroupResponse>>>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    
    public string? Search { get; set; }
}

public sealed class GetAllDayGroupsRequestValidation : AbstractValidator<GetAllDayGroupsRequest>
{
    public GetAllDayGroupsRequestValidation()
    {
        RuleFor(s => s.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(s => s.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");
    }
}