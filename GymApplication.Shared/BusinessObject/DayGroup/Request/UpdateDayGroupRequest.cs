using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.DayGroup.Request;

public sealed class UpdateDayGroupRequestValidation : AbstractValidator<UpdateDayGroupRequest>
{
    public UpdateDayGroupRequestValidation()
    {
        RuleFor(d => d.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must not be empty");
        
        RuleFor(d => d.Group)
            .NotNull().NotEmpty()
            .WithMessage("Group must not be null or empty");
    }
}

public sealed class UpdateDayGroupRequest : IRequest<Result>
{ 
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public string Group { get; set; }
}