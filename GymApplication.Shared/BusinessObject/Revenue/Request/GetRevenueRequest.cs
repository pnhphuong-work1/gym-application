using FluentValidation;
using GymApplication.Shared.BusinessObject.Revenue.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Revenue.Request;

public sealed class GetRevenueRequestValidator : AbstractValidator<GetRevenueRequest>
{
    public GetRevenueRequestValidator()
    {
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}

public sealed class GetRevenueRequest : IRequest<Result<RevenueResponse>>
{
    public int Month { get; set; }
    public int Year { get; set; }
}