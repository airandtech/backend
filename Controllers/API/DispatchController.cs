using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AutoMapper;
using AirandWebAPI.Models;
using AirandWebAPI.Helpers;
using AirandWebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AirandWebAPI.Core.Domain;

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

        [HttpGet("accept/{email}")]
        public async Task<IActionResult> Accept(string email)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var user = (User)HttpContext.Items["User"];
                    int userId = user.Id;
                    bool response = await _orderService.Accept(email, userId);
                    if (response)
                        return Ok(new GenericResponse<string>(true, ResponseMessage.SUCCESSFUL, ResponseMessage.SUCCESSFUL));
                    return Ok(new GenericResponse<string>(false, ResponseMessage.FAILED, ResponseMessage.FAILED));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED, "Email is required");
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURED);
                return StatusCode(500, exceptionHandler);

            }
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            await _orderService.test();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> FlutterWaveWebHook([FromBody] FluttterwaveResponse response)
        {
            try
            {
                bool resp = await _orderService.ReceivePayment(response);
                if (resp) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURED);
                return StatusCode(500, exceptionHandler);
            }
        }

    }
}