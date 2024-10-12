using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed class UpdateSubscriptionRequestValidation : AbstractValidator<UpdateSubscriptionRequest>
{
    public UpdateSubscriptionRequestValidation()
    {
        RuleFor(s => s.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must not be empty");
        
        RuleFor(s => s.Group)
            .NotNull().NotEmpty()
            .WithMessage("Group must not be null or empty");
        
        RuleFor(s => s.Name)
            .NotNull().NotEmpty();

        RuleFor(s => s.Price)
            .GreaterThan(0).NotNull();

        RuleFor(s => s.TotalWorkoutTime)
            .NotNull();
    }
}

public sealed class UpdateSubscriptionRequest : IRequest<Result>
{ 
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TimeSpan TotalWorkoutTime { get; set; }
    public decimal Price { get; set; }
    public string Group { get; set; }
}