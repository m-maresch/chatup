using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class UpdateUserTagsCommand
    {
        public int UserID { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
