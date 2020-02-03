using System.Collections.Generic;

namespace Common
{
    public class SetUserContactsRequestDto
    {
        public int UserID { get; set; }
        public List<SetUserContactsPhoneNumberDto> PhoneNumbers { get; set; }
    }
}