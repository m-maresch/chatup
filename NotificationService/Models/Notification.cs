using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Models
{
    public class Notification
    {
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string MessageContent { get; set; }
        public DateTime Time { get; set; }
    }
}
