using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MockServer.Controllers
{
    [Route("/[controller]")]
    public class ConnectionsController : Controller
    {
        private IHubContext<NotificationsHub> _hubContext;

        public ConnectionsController(IHubContext<NotificationsHub> hubContext)
        {
            this._hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] NotificationsConnectionDto message)
        {
            await _hubContext.Groups.AddAsync(message.ConnectionID, message.UserID.ToString());
            
            return Ok();
        }
        
        [HttpDelete]
        public async Task<IActionResult> Unregister([FromBody] NotificationsConnectionDto message)
        {
            await _hubContext.Groups.RemoveAsync(message.ConnectionID, message.UserID.ToString());

            return Ok();
        }
    }

}
