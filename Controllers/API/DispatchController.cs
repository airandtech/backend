using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AutoMapper;
using AirandWebAPI.Models.Dispatch;
using AirandWebAPI.Helpers;
using AirandWebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models;

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
        private ISmsService _smsService;
        public DispatchController(
            IOrderService orderService,
            IMapper mapper,
            IValidation<RideOrderRequest> dispatchRequestValidation,
            ISmsService smsService
        )
        {
            _orderService = orderService;
            _mapper = mapper;
            _dispatchRequestValidation = dispatchRequestValidation;
            _smsService = smsService;
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] RideOrderRequest model)
        {
            try
            {
                ValidationInfo validationInfo = _dispatchRequestValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    DispatchResponse response = await _orderService.Order(model);
                    if(response != null)
                        return Ok(new GenericResponse<DispatchResponse>(true, ResponseMessage.SUCCESSFUL, response));
                    return Ok(new GenericResponse<DispatchResponse>(false, ResponseMessage.SUCCESSFUL, response));
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

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsBody model)
        {
            try
            {
                if (model != null && (!string.IsNullOrWhiteSpace(model.from) || !string.IsNullOrWhiteSpace(model.to) || !string.IsNullOrWhiteSpace(model.message)))
                {
                    bool response = await _smsService.SendAsync(model);
                    return Ok(new GenericResponse<string>(true, ResponseMessage.SUCCESSFUL, "Processing"));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED, "Bad Request");
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