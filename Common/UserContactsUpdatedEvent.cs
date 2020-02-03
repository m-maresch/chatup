using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class UserContactsUpdatedEvent
    {
        public int UserID { get; set; }
        public List<int> ContactIds { get; set; }
    }
}
