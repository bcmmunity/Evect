namespace Evect.Models
{
    public class UserEvent
    {
        public int UserEventId { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}