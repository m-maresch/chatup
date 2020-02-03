using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MockServer.Controllers
{
    [Route("/[controller]")]
    //[Authorize]
    public class MessagesController : Controller
    {
        private IHubContext<NotificationsHub> _hubContext;

        public MessagesController(IHubContext<NotificationsHub> hubContext)
        {
            this._hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto message)
        {
            await _hubContext.Clients.Group(message.ReceiverID.ToString()).SendAsync(new
            {
                SenderID = 11,
                MessageContent = "Hi Michael",
                Time = "05.12.2017 22:22:22"
            }.ToJson());

            return Ok();
        }
    }
}
