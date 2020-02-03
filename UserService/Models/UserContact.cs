using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class UserContact
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int ContactID { get; set; }
    }
}
