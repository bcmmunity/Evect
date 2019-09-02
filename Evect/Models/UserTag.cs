namespace Evect.Models
{
    public class UserTag
    {
        public int UserTagId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
    public class UserSearchingTag
    {
        public int UserSearchingTagId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public SearchingTag Tag { get; set; }
        public int TagId { get; set; }
    }
    
}