using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class UpdateUserRequestValidation : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidation()
    {
        RuleFor(u => u.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must not be empty");
        
        RuleFor(u => u.FullName)
            .NotNull().NotEmpty()
            .WithMessage("FullName must not be null or empty");

        RuleFor(u => u.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .NotNull()
            .WithMessage("DateOfBirth must not be null or empty");
        
        RuleFor(u => u.Email)
            .EmailAddress()
            .NotNull()
            .NotEmpty()
            .WithMessage("Email must not be null or empty");
        
        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .NotNull()
            .WithMessage("PhoneNumber must not be null or empty");
    }
}

public sealed class UpdateUserRequest : IRequest<Result>
{ 
    [NotMapped]
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
}