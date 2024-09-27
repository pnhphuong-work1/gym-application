using FluentValidation;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class GetAllUserRequestValidation : AbstractValidator<GetAllUserRequest>
{
    public GetAllUserRequestValidation()
    {
        RuleFor(u => u.CurrentPage)
            .GreaterThan(0);

        RuleFor(u => u.PageSize)
            .GreaterThan(0);

        RuleFor(u => u.SortBy)
            .Must(u => u is "fullName" or "email" or "phoneNumber" or "dateOfBirth");

        RuleFor(u => u.SortOrder)
            .Must(u => u is "asc" or "desc");
        
        RuleFor(u => u.SearchBy)
            .Must(u => u is "fullName" or "email" or "phoneNumber" or "dateOfBirth");
    }
}

public sealed class GetAllUserRequest : IRequest<Result<PagedResult<UserResponse>>>
{
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "email";
    public string SortOrder { get; set; } = "asc";
    public string? SortBy { get; set; } = "email";
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}