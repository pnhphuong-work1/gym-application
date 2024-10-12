using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed class UpdateCheckLogsRequestValidation : AbstractValidator<UpdateCheckLogsRequest>
{
    public UpdateCheckLogsRequestValidation()
    {
        RuleFor(u => u.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must not be empty");
        
        // RuleFor(u => u.CheckInId)
        //     .NotNull()
        //     .NotEqual(Guid.Empty)
        //     .WithMessage("CheckInId must not be empty");
        //
        // RuleFor(u => u.UserSubscriptionId)
        //     .NotEqual(Guid.Empty)
        //     .WithMessage("UserSubscriptionId must not be empty");
        
        RuleFor(u => u.CheckStatus)
            .NotNull()
            .NotEmpty()
            .WithMessage("CheckStatus must not be null or empty");
        
        RuleFor(u => u.WorkoutTime)
            .NotNull()
            .WithMessage("WorkoutTime must not be null or empty");
    }
}

public sealed class UpdateCheckLogsRequest : IRequest<Result>
{
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    //public Guid UserId { get; set; }
    //public Guid? CheckInId { get; set; }
    //public Guid UserSubscriptionId { get; set; }
    public string? CheckStatus { get; set; }
    public TimeOnly? WorkoutTime { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}