using System.ComponentModel.DataAnnotations.Schema;

namespace Evect.Models
{
    public class MobileUser
    {
        public int MobileUserId { get; set; }
        public string Email { get; set; }
        public string EmailCode { get; set; }
        public long TelegramId { get; set; }
        public string ApiKey { get; set; }
    }
}