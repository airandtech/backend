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
    public class EmailController : ControllerBase
    {
        private IMailer _mailerService;
        private IValidation<EmailVM> _sendEmailValidation;
        public EmailController(
            IMailer mailerService,
            IValidation<EmailVM> sendEmailValidation
        )  
        {
            _mailerService = mailerService;
            _sendEmailValidation = sendEmailValidation;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailVM model)
        {
            try
            {
                
                ValidationInfo validationInfo = _sendEmailValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    MailResp resp = await _mailerService.SendMailAsync(model.to, model.name, model.subject, model.message);
                    if (resp.status)
                        return Ok(new GenericResponse<MailResp>(true, ResponseMessage.SUCCESSFUL, resp));

                    return BadRequest(new GenericResponse<MailResp>(false, ResponseMessage.FAILED, resp));
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