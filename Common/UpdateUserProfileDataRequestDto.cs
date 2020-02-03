using System.Collections.Generic;

namespace Common
{
    public class UpdateUserProfileDataRequestDto
    {
        public int UserID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string OldPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public string OldBiography { get; set; }
        public string NewBiography { get; set; }
        public List<TagDto> OldTags { get; set; }
        public List<TagDto> NewTags { get; set; }
    }
}