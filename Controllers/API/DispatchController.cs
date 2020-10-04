using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AutoMapper;
using AirandWebAPI.Models;
using AirandWebAPI.Helpers;
using AirandWebAPI.Exceptions;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchController : ControllerBase
    {
        private IOrderService _orderService;
        private IMapper _mapper;
        private IValidation<RideOrderRequest> _dispatchRequestValidation;
        public DispatchController(
            IOrderService orderService,
            IMapper mapper,
            IValidation<RideOrderRequest> dispatchRequestValidation
        )
        {
            _orderService = orderService;
            _mapper = mapper;
            _dispatchRequestValidation = dispatchRequestValidation;
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] RideOrderRequest model)
        {
            try
            {
                ValidationInfo validationInfo = _dispatchRequestValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    bool response = await _orderService.Order(model);
                    return Ok(new GenericResponse<string>(true, ResponseMessage.SUCCESSFUL, "Processing"));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED, validationInfo.getConcatInvalidationNarrations());
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURED);
                return StatusCode(500, exceptionHandler);

            }
        }

        
    }
}