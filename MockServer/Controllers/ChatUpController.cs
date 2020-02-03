using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.Controllers
{
    [Route("/[controller]")]
    //[Authorize]
    public class ChatUpController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult ChatUp(long id)
        {
            return Ok(new ChatUpResponseDto()
            {
                UserID = 1,
                UserName = "Berger"
            });
        }
    }
}
