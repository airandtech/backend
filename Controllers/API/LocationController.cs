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
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private ILocationService _locationService;
        private IMapper _mapper;
        private IValidation<Coordinates> _coordinatesDataValidation;
        public LocationController(
            ILocationService locationService,
            IMapper mapper,
            IValidation<Coordinates> coordinatesDataValidation
        )
        {
            _locationService = locationService;
            _mapper = mapper;
            _coordinatesDataValidation = coordinatesDataValidation;
        }

        [HttpPost("driver/update")]
        public async Task<IActionResult> UpdateDriverLocation([FromBody] Coordinates model)
        {
            try
            {
                ValidationInfo validationInfo = _coordinatesDataValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    var user = (User)HttpContext.Items["User"];
                    int userId = user.Id;
                    bool response = await _locationService.UpdateDriverLocation(model, userId);
                    if(response)
                        return Ok(new GenericResponse<string>(true, ResponseMessage.SUCCESSFUL, "Successful"));
                    return Ok(new GenericResponse<string>(false, ResponseMessage.FAILED, "Failed"));
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