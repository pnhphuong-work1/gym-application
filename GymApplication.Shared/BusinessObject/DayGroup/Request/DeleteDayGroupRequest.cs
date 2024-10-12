using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.DayGroups.Request;

public sealed record DeleteDayGroupRequest(Guid Id) : IRequest<Result>;