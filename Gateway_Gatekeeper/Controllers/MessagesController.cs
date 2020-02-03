using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Gateway_Gatekeeper.Extensions;
using Gateway_Gatekeeper.Validator;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Polly;

namespace Gateway_Gatekeeper.Controllers
{
    [Route("/[controller]")]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly Policy _policy;

        public MessagesController(IPublishEndpoint publishEndpoint, Policy policy)
        {
            this._publishEndpoint = publishEndpoint;
            this._policy = policy;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto message)
        {
            if (await message.PublishWhenValid(_publishEndpoint))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
