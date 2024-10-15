using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Request;

public sealed record DeleteSubscriptionUserRequest(Guid Id) : IRequest<Result>;