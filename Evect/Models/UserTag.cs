namespace Evect.Models
{
    public class UserTag
    {
        public int UserTagId { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }
        public bool ForSearching { get; set; }
    }
}