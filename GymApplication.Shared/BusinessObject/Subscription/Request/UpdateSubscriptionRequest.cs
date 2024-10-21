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
    }
}

public sealed class UpdateSubscriptionRequest : IRequest<Result>
{ 
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? TotalWorkoutTime { get; set; }
    public decimal Price { get; set; }
    public int TotalMonth { get; set; }
    public string? Group { get; set; }
}