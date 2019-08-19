namespace Evect.Models
{
    public class EventCode
    {
        public int EventCodeId { get; set; }
        public string Code { get; set; }
        public bool IsForOrganizer { get; set; } = false;
    }
}