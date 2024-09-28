using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed record DeleteUserRequest(Guid Id) : IRequest<Result>;