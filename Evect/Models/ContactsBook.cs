namespace Evect.Models
{
    public class ContactsBook
    {
        public int ContactsBookId { get; set; }
        public bool IsAccepted { get; set; }
        public long OwnerId { get; set; }
        public long AnotherUserId { get; set; }
    }
}