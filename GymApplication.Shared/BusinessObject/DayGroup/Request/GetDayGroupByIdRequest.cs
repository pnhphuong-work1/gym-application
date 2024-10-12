using FluentValidation;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.DayGroups.Request;

public sealed record GetDayGroupById(Guid Id) : IRequest<Result<DayGroupResponse>>;
public sealed class GetDayGroupByIdValidation : AbstractValidator<GetDayGroupById> {
    public GetDayGroupByIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}