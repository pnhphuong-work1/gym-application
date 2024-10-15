using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class GetCustomerById : IRequest<Result<CustomerResponse>>
{
    public Guid Id { get; set; }
}