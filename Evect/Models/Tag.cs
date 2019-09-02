using System.Collections.Generic;

namespace Evect.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public int ParentTagID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public List<UserTag> UserTags { get; set; }
        
    }
}