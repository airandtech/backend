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
        public CompanyController(
            ICompanyService companyService,
            IMapper mapper,
            IValidation<CreateCompanyVM> createCompanyValidation
        )
        {
            _companyService = companyService;
            _mapper = mapper;
            _createCompanyValidation = createCompanyValidation;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyVM model)
        {
            try
            {
                var company = _mapper.Map<Company>(model);

                var user = (User)HttpContext.Items["User"];
                int userId = user.Id;
                ValidationInfo validationInfo = _createCompanyValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    company = await _companyService.Create(company, userId);
                    if (company != null)
                        return Ok(new GenericResponse<Company>(true, ResponseMessage.SUCCESSFUL, company));
                    
                    return Ok(new GenericResponse<Company>(false, ResponseMessage.FAILED, null));
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