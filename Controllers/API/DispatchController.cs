using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirandWebAPI.Services.Contract;

namespace AirandWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchController : ControllerBase
    {
        private IOrderService _orderService;
        public DispatchController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        
    }
}