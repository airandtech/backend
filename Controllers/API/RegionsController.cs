using Microsoft.AspNetCore.Mvc;
using AirandWebAPI.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RegionsController : ControllerBase
    {
        private IRegionService _regionService;
        private IMapper _mapper;
        public RegionsController(
            IRegionService regionService,
            IMapper mapper
        )
        {
            _regionService = regionService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet()]
        public IActionResult GetAll()
        {
            var users = _regionService.GetAll();
            return Ok(users);
        }

    }
}