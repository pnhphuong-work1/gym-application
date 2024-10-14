using Asp.Versioning;
using GymApplication.api.Common;
using GymApplication.Shared.BusinessObject.Payment.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Controllers;

[ApiController]
[ApiVersion("2024-09-29")]
[Route("api/v{version:apiVersion}/payments")]
public class PaymentController : RestController
{
    private readonly ISender _sender;

    public PaymentController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        var result = await _sender.Send(request);
        
        return result.IsSuccess ? 
            Results.Ok(result)
            : HandlerFailure(result);
    }
    
    [HttpPost("payment-return")]
    public async Task<IResult> PaymentReturn([FromBody] PaymentReturnRequest request)
    {
        var result = await _sender.Send(request);
        
        return result.IsSuccess ? 
            Results.Ok(result)
            : HandlerFailure(result);
    }
    
    [HttpGet]
    public async Task<IResult> GetPayments([FromQuery] GetAllPaymentRequest request)
    {
        var result = await _sender.Send(request);
        
        return result.IsSuccess ? 
            Results.Ok(result)
            : HandlerFailure(result);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IResult> GetPaymentByUserId(GetPaymentByUserId request)
    {
        var result = await _sender.Send(request);
        
        return result.IsSuccess ? 
            Results.Ok(result)
            : HandlerFailure(result);
    }
    
    // [HttpGet("{id}")]
    // public async Task<IResult> GetPaymentById([FromRoute] Guid id)
    // {
    //     var request = new GetPaymentByIdRequest { Id = id };
    //     var result = await _sender.Send(request);
    //     
    //     return result.IsSuccess ? 
    //         Results.Ok(result) 
    //         : HandlerFailure(result);
    // }   
}