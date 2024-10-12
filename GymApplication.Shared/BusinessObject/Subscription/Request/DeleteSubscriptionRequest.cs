using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.Subscription.Request;

public sealed record DeleteSubscriptionRequest(Guid Id) : IRequest<Result>;