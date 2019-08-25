namespace Evect.Models
{
    public class UserValidation
    {
        public int UserValidationId { get; set; }
        public long UserTelegramId { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        
    }
}