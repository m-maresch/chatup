using System.Collections.Generic;

namespace Common
{
    public class GetUserProfileDataResponseDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<TagDto> Tags { get; set; }
        public string Biography { get; set; }
    }
}