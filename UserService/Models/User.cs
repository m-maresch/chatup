using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Biography { get; set; }
        public ICollection<UserContact> Contacts { get; set; }
    }
}
