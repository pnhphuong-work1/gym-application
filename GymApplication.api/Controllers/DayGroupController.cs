using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.DayGroup.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Request;
using GymApplication.Shared.BusinessObject.DayGroups.Respone;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiVersion("2024-09-19")]
[Route("api/v{version:apiVersion}/dayGroups")]
public class DayGroupController : RestController
{
    private readonly ISender _mediator;

    public DayGroupController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Result<PagedResult<DayGroupResponse>>))]
    public async Task<IResult> Get([FromQuery] GetAllDayGroupsRequest request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : HandlerFailure(result);
    }
    
    [HttpGet("{id:guid}", Name = "GetDayGroupById")]
    [ProducesResponseType(200, Type = typeof(Result<DayGroupResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> GetDayGroupById(Guid id)
    {
        var request = new GetDayGroupById(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Result<DayGroupResponse>))]
    [ProducesResponseType(400, Type = typeof(Result))]
    public async Task<IResult> Post([FromBody] CreateDayGroupRequest request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
            ? Results.CreatedAtRoute("GetDayGroupById", new { id = result.Value.Id }, result)
            : HandlerFailure(result);
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(Result<DayGroupResponse>))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Put(Guid id, [FromBody] UpdateDayGroupRequest request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(Result))]
    [ProducesResponseType(404, Type = typeof(Result))]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var request = new DeleteDayGroupRequest(id);
        var result = await _mediator.Send(request);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : HandlerFailure(result);
    }
}