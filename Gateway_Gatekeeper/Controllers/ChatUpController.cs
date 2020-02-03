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
using Polly;

namespace Gateway_Gatekeeper.Controllers
{
    [Route("/[controller]")]
    [Authorize]
    public class ChatUpController : Controller
    {
        private readonly IRequestClient<ChatUpRequestDto, ChatUpResponseDto> _chatUpClient;
        private readonly Policy _policy;

        public ChatUpController(IRequestClient<ChatUpRequestDto, ChatUpResponseDto> chatUpClient, Policy policy)
        {
            this._chatUpClient = chatUpClient;
            this._policy = policy;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ChatUp(long id)
        {
            ChatUpRequestDto message = new ChatUpRequestDto()
            {
                UserID = (int)id
            };
            
            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _chatUpClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }
    }
}
