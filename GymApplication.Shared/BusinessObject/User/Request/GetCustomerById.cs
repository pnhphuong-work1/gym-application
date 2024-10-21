using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed record GetCustomerById(Guid Id) : IRequest<Result<CustomerResponse>>;