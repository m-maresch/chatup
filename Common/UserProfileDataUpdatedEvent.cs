using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class UserProfileDataUpdatedEvent
    {
        public int UserID { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Biography { get; set; }
    }
}
