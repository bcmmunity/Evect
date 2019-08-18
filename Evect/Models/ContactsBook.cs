namespace Evect.Models
{
    public class ContactsBook
    {
        public int ContactsBookId { get; set; }
        public bool IsAccepted { get; set; }
        public int FirstUserId { get; set; }
        public int SecondUserId { get; set; }
    }
}