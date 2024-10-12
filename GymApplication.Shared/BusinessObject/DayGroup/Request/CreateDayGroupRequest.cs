using FluentValidation;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.DayGroups.Request;

public sealed class CreateDayGroupRequest : IRequest<Result<DayGroupResponse>>
{
    public string Group{ get; set; }
}
public sealed class CreateDayGroupRequestValidation : AbstractValidator<CreateDayGroupRequest> {
    public CreateDayGroupRequestValidation()
    {
        RuleFor(d => d.Group)
            .NotNull().NotEmpty();
    }
}