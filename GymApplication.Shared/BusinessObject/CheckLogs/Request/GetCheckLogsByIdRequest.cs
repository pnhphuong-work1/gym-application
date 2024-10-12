using FluentValidation;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed class GetCheckLogByIdValidation : AbstractValidator<GetCheckLogsByIdRequest>
{
    public GetCheckLogByIdValidation()
    {
        RuleFor(u => u.Id)
            .NotEmpty();
    }
}

public sealed record GetCheckLogsByIdRequest(Guid Id) : IRequest<Result<CheckLogsResponse>>;