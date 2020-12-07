using System.Diagnostics;
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
using AirandWebAPI.Models.Response;
using Microsoft.Extensions.Logging;

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
        private IValidation<ChangeStatusVM> _changeStatusValidation;
        private ISmsService _smsService;
        private readonly ILogger _logger;
        public DispatchController(
            IOrderService orderService,
            IMapper mapper,
            IValidation<RideOrderRequest> dispatchRequestValidation,
            ISmsService smsService,
            IValidation<ChangeStatusVM> changeStatusValidation,
            ILogger<DispatchController> logger
        )
        {
            _orderService = orderService;
            _mapper = mapper;
            _dispatchRequestValidation = dispatchRequestValidation;
            _smsService = smsService;
            _changeStatusValidation = changeStatusValidation;
            _logger = logger;
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
                    if (response != null)
                        return Ok(new GenericResponse<DispatchResponse>(true, ResponseMessage.SUCCESSFUL, response));
                    return Ok(new GenericResponse<DispatchResponse>(false, ResponseMessage.FAILED, response));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED, validationInfo.getConcatInvalidationNarrations());
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
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
                    return Ok(new GenericResponse<string>(false, "Order is no longer available", ResponseMessage.FAILED));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, "Email is required", ResponseMessage.FAILED);
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
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
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                _logger.LogInformation(json);
            
                bool resp = await _orderService.ReceivePayment(response);
                if (resp) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
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
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);

            }
        }

        [HttpGet("orders/fetch")]
        public IActionResult GetOrders()
        {
            try
            {
                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;

                RiderOrders riderOrders = _orderService.GetOrders(userId);
                if (riderOrders != null && (riderOrders.completed.Count > 0 || riderOrders.pending.Count > 0 || riderOrders.inProgress.Count > 0))
                {
                    return Ok(new GenericResponse<RiderOrders>(true, ResponseMessage.SUCCESSFUL, riderOrders));
                }
                return Ok(new GenericResponse<RiderOrders>(false, ResponseMessage.NO_RESULTS, null));

            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);

            }
        }

        [HttpPost("orders/status/change")]
        public IActionResult ChangeStatus([FromBody] ChangeStatusVM model)
        {
            try
            {
                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                ValidationInfo validationInfo = _changeStatusValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    bool isSuccessful = _orderService.ChangeStatus(model);
                    if (isSuccessful)
                    {
                         return Ok(new GenericResponse<string>(true, ResponseMessage.SUCCESSFUL, ResponseMessage.SUCCESSFUL));
                    }

                    return Ok(new GenericResponse<string>(false, ResponseMessage.FAILED, ResponseMessage.FAILED));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED, validationInfo.getConcatInvalidationNarrations());
                    return BadRequest(errorResponse);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);

            }
        }

    }
}