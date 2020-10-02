using AirandWebAPI.Models.Auth;
using AirandWebAPI.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models;
using System;
using AirandWebAPI.Exceptions;
using AirandWebAPI.Helpers;
using AirandWebAPI.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private IValidation<RegisterModel> _userValidation;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IValidation<RegisterModel> userValidation)
        {
            _userService = userService;
            _mapper = mapper;
            _userValidation = userValidation;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);
            try
            {
                ValidationInfo validationInfo = _userValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    AuthenticateResponse response = _userService.Create(user, model.Password);
                    return Ok(response);
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.REGISTRATION_FAILED, validationInfo.getConcatInvalidationNarrations());
                    return Ok(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURED);
                return Ok(exceptionHandler);

            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}