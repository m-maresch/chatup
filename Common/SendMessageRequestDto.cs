using System;

namespace Common
{
    public class SendMessageRequestDto
    {
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string MessageContent { get; set; }
        public DateTime Time { get; set; }
    }
}