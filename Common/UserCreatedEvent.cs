﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class UserCreatedEvent
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
