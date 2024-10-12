using GymApplication.Shared.Common;
using MediatR;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed record DeleteCheckLogsRequest(Guid Id) : IRequest<Result>;