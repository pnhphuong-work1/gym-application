using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Newtonsoft.Json;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class CreateUserRequestValidation : AbstractValidator<CreateUserRequest> {
    public CreateUserRequestValidation()
    {
        RuleFor(u => u.FullName)
            .NotNull().NotEmpty();

        RuleFor(u => u.Password)
            .Length(6, 20).NotNull();

        RuleFor(u => u.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .NotNull();
        
        RuleFor(u => u.Email)
            .EmailAddress()
            .NotNull();
        
        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .NotNull();
        
        RuleFor(u => u.Role)
            .NotEmpty()
            .NotNull();
        
        RuleFor(u => u.ConfirmPassword)
            .Matches(u => u.Password)
            .WithMessage("ConfirmPassword and Password don't match");
    }
}

public sealed class CreateUserRequest : IRequest<Result<UserResponse>>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    [JsonIgnore]
    [SwaggerIgnore]
    public Role Role { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}