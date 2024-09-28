using GymApplication.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Common;

[ApiController]
public abstract class RestController : ControllerBase
{
    protected static IResult HandlerFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            { Error.Code: "400" } => Results.BadRequest(
                CreateProblemDetails(
                    "Bad Request", StatusCodes.Status400BadRequest,
                    result.Error)),
            { Error.Code: "404" } => Results.NotFound(
                CreateProblemDetails(
                    "Not Found", StatusCodes.Status404NotFound,
                    result.Error)),
            { Error.Code: "500" } => Results.BadRequest(
                CreateProblemDetails(
                    "Internal Server Error", StatusCodes.Status500InternalServerError,
                    result.Error)),
            IValidationResult validationResult =>
                Results.BadRequest(
                    CreateProblemDetails(
                        "Validation Error", StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors)),
            _ =>
                Results.BadRequest(
                    CreateProblemDetails(
                        "Bab Request", StatusCodes.Status400BadRequest,
                        result.Error))
        };

    private static ProblemDetails CreateProblemDetails(string title, int status, Error error, Error[]? errors = null)
        => new()
        {
            Title = title,
            Type = error.Code,
            Detail = error.Message,
            Status = status,
            Extensions = { { nameof(errors), errors } }
        };
}