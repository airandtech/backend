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
using AirandWebAPI.Models.Merchant;
using System.Collections.Generic;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MerchantsController : ControllerBase
    {
        private IMerchantService _merchantService;
        private IMapper _mapper;
        private IValidation<RegisterMerchantVM> _registerMerchantValidation;
        public MerchantsController(
            IMerchantService merchantService,
            IMapper mapper,
            IValidation<RegisterMerchantVM> registerMerchantValidation
        )
        {
            _merchantService = merchantService;
            _mapper = mapper;
            _registerMerchantValidation = registerMerchantValidation;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var merchants = _merchantService.GetAll();
                if (merchants != null)
                    return Ok(new GenericResponse<List<Merchant>>(true, ResponseMessage.SUCCESSFUL, merchants));

                return BadRequest(new GenericResponse<Merchant>(false, ResponseMessage.FAILED, null));

            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var merchant = _merchantService.GetById(id);
                if (merchant != null)
                    return Ok(new GenericResponse<Merchant>(true, ResponseMessage.SUCCESSFUL, merchant));

                return BadRequest(new GenericResponse<Merchant>(false, ResponseMessage.FAILED, null));

            }
            catch (Exception ex)
            {
                ExceptionHandler exceptionHandler = new ExceptionHandler(false, ex, ResponseMessage.EXCEPTION_OCCURRED);
                return StatusCode(500, exceptionHandler);
            }
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] RegisterMerchantVM model)
        {
            try
            {
                var merchant = _mapper.Map<Merchant>(model);

                ValidationInfo validationInfo = _registerMerchantValidation.Validate(model);
                if (validationInfo.isValid())
                {
                    merchant = await _merchantService.Create(merchant);
                    if (merchant != null)
                        return Ok(new GenericResponse<Merchant>(true, ResponseMessage.SUCCESSFUL, merchant));

                    return BadRequest(new GenericResponse<Merchant>(false, ResponseMessage.FAILED, null));
                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(false, validationInfo.getConcatInvalidationNarrations(), ResponseMessage.FAILED);
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