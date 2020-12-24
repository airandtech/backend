using Microsoft.AspNetCore.Mvc;
using AirandWebAPI.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Company;
using AirandWebAPI.Services;
using AirandWebAPI.Helpers;
using AirandWebAPI.Models;
using System;
using AirandWebAPI.Exceptions;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private ICompanyService _companyService;
        private IMapper _mapper;
        private IValidation<CreateCompanyVM> _createCompanyValidation;
        private IValidation<AddDispatchManagerVM> _addDispatchManagerValidation;
        private IValidation<AddRidersVM> _addRidersValidation;
        public CompanyController(
            ICompanyService companyService,
            IMapper mapper,
            IValidation<CreateCompanyVM> createCompanyValidation,
            IValidation<AddDispatchManagerVM> addDispatchManagerValidation,
            IValidation<AddRidersVM> addRidersValidation
        )
        {
            _companyService = companyService;
            _mapper = mapper;
            _createCompanyValidation = createCompanyValidation;
            _addDispatchManagerValidation = addDispatchManagerValidation;
            _addRidersValidation = addRidersValidation;
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] CreateCompanyVM model)
        {
            try
            {
                var company = _mapper.Map<Company>(model);

                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                model.UserId = userId;
                ValidationInfo validationInfo = _createCompanyValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    company = await _companyService.Create(company, userId);
                    if (company != null)
                        return Ok(new GenericResponse<Company>(true, ResponseMessage.SUCCESSFUL, company));

                    return BadRequest(new GenericResponse<Company>(false, ResponseMessage.FAILED, null));
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

        [HttpPost("managers/")]
        public async Task<IActionResult> AddDispatchManagers([FromBody] AddDispatchManagerVM model)
        {
            try
            {
                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                ValidationInfo validationInfo = _addDispatchManagerValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    bool isSuccessful = await _companyService.AddDispatchManagers(model, userId);
                    if (isSuccessful)
                        return Ok(new GenericResponse<string>(true, "Dispatch Manager(s) added successfully !!!", ResponseMessage.SUCCESSFUL));

                    return BadRequest(new GenericResponse<string>(false, "Error occurred in processing", ResponseMessage.FAILED));
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

        [HttpPost("riders/")]
        public async Task<IActionResult> AddRiders([FromBody] AddRidersVM model)
        {
            try
            {
                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                ValidationInfo validationInfo = _addRidersValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    bool isSuccessful = await _companyService.AddRiders(model, userId);
                    if (isSuccessful)
                        return Ok(new GenericResponse<string>(true, "Rider(s) added successfully !!!", ResponseMessage.SUCCESSFUL));

                    return BadRequest(new GenericResponse<string>(false, "Error occurred in processing", ResponseMessage.FAILED));
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

        [HttpDelete("rider/{id}")]
        public async Task<IActionResult> DeleteRider(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool isSuccessful = await _companyService.DeleteRider(id);
                    if (isSuccessful)
                        return Ok(new GenericResponse<string>(true, "Rider deleted !!!", ResponseMessage.SUCCESSFUL));

                    return BadRequest(new GenericResponse<string>(false, "Error occurred in processing", ResponseMessage.FAILED));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, "Invalid Rider Id" , ResponseMessage.FAILED);
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);
            }
        }

        [HttpPost("createWithDetails/")]
        public async Task<IActionResult> CreateCompanyWithDetails([FromBody] CompanyWithDetailsVM model)
        {
            try
            {
                AddRidersVM riderModel = new AddRidersVM();
                riderModel.ridersDetails = model.ridersDetails;

                AddDispatchManagerVM dispatchModel = new AddDispatchManagerVM();
                dispatchModel.managerDetails = model.managerDetails;

                CreateCompanyVM companyModel = model.company;

                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                model.company.UserId = userId;

                ValidationInfo riderValidationInfo = _addRidersValidation.Validate(riderModel);
                ValidationInfo companyValidationInfo = _createCompanyValidation.Validate(companyModel);
                ValidationInfo dispatchValidationInfo = _addDispatchManagerValidation.Validate(dispatchModel);

                if (riderValidationInfo.isValid() && companyValidationInfo.isValid() && dispatchValidationInfo.isValid())
                {
                    CompanyWithDetailsVM companyWithDetailsVM = await _companyService.CreateCompanyWithDetails(model, userId);
                    if (companyWithDetailsVM != null)
                        return Ok(new GenericResponse<CompanyWithDetailsVM>(true, "Company added successfully !!!", companyWithDetailsVM));

                    return BadRequest(new GenericResponse<string>(false, "Error occurred in processing", ResponseMessage.FAILED));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, ResponseMessage.FAILED,
                    $" {riderValidationInfo.getConcatInvalidationNarrations()} - {companyValidationInfo.getConcatInvalidationNarrations()} - {dispatchValidationInfo.getConcatInvalidationNarrations()}"
                    );
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);
            }
        }

        [HttpGet("user/")]
        public async Task<IActionResult> GetCompanyDetails()
        {
            try
            {
                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;


                UserCompanyRider userCompanyRider = _companyService.GetCompanyDetails(userId);
                if (userCompanyRider != null)
                    return Ok(new GenericResponse<UserCompanyRider>(true, ResponseMessage.SUCCESSFUL, userCompanyRider));

                return BadRequest(new GenericResponse<string>(false, "Error occurred in processing", ResponseMessage.FAILED));

            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);
            }
        }

    }
}