using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Revenue.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiController]
[ApiVersion("2024-09-29")]
[Route("api/v{version:apiVersion}/revenues")]
public class RevenueController : RestController
{
    private readonly ISender _sender;

    public RevenueController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<IResult> GetRevenue([FromQuery] GetRevenueRequest request)
    {
        var result = await _sender.Send(request);
        
        return result.IsSuccess ? 
            Results.Ok(result)
            : HandlerFailure(result);
    }
}