using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Controllers
{
    [Route("/[controller]")]
    public class MessagesController : Controller
    {
        private IHubContext<NotificationsHub> _hubContext;
        private IRepository<User> _userRepository;

        public MessagesController(IHubContext<NotificationsHub> hubContext, IRepository<User> userRepository)
        {
            _hubContext = hubContext;
            _userRepository = userRepository;
        }

        [HttpPost("send")]
        public async Task<IActionResult> ChatUp([FromBody] SendMessageRequestDto message)
        {
            User target = await _userRepository.GetById(message.ReceiverID);
            try
            {
                await _hubContext.Clients.Group(target.PhoneNumber).SendAsync(new
                {
                    SenderID = message.SenderID,
                    MessageContent = message.MessageContent,
                    Time = message.Time
                }.ToJson());
            }
            catch 
            {
                //Generate Key and store in Redis 
            }

            return Ok();
        }
    }
}
