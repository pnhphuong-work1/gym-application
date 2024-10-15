using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed class UpdateSubscriptionUserValidation : AbstractValidator<UpdateSubscriptionUserRequest>
{
    public UpdateSubscriptionUserValidation()
    {
        RuleFor(s => s.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must not be empty");
    }
}
    

public sealed class UpdateSubscriptionUserRequest : IRequest<Result>
{
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public decimal? PaymentPrice { get; set; }
    public int? WorkoutSteak { get; set; }
    public int? LongestWorkoutSteak { get; set; }
    public DateTime? LastWorkoutDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}