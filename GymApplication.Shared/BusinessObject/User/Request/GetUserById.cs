using FluentValidation;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class GetUserByIdValidation : AbstractValidator<GetUserById> {
    public GetUserByIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}

public sealed record GetUserById(Guid Id) : IRequest<Result<UserResponse>>;