using System.Net.Mime;
using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.CheckLogs.Request;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/check-logs")]
[Produces(MediaTypeNames.Application.Json)]
public class CheckLogsController : RestController
{
    private readonly ISender _mediator;

    public CheckLogsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Result<PagedResult<CheckLogsResponse>>))]
    //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.User)}")]
    public async Task<IResult> Get([FromQuery] GetAllCheckLogsRequest request)
    {
        var result = await _mediator.Send(request);
        
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetCheckLogById")]
    [ProducesResponseType(200, Type = typeof(Result<CheckLogsResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.User)}")]
    public async Task<IResult> GetCheckLogById(Guid id)
    {
        var request = new GetCheckLogsByIdRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    [ProducesResponseType(201, Type = typeof(Result<CheckLogsResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Post(CreateCheckLogsRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.CreatedAtRoute("GetCheckLogById", new { id = result.Value.Id }, result) 
            : HandlerFailure(result);
    }
    
    [HttpPut("{id:guid}")]
    //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    [ProducesResponseType(200, Type = typeof(Result))]
    [Authorize(Roles = $"{nameof(Role.Manager)}")]
    public async Task<IResult> Put(Guid id, UpdateCheckLogsRequest request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Manager)}")]
    //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var request = new DeleteCheckLogsRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
}