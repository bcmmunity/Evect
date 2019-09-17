using System.Collections.Generic;
using Evect.Models;
namespace EvectCorp.Models
{
    public class AdminUser
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }

        public Actions CurrentAction { get; set; } = Actions.None;
        
        public bool IsAdmin { get; set; }
        
        public string TempEventName { get; set; }
        public string TempEventCode { get; set; }
        public int TempParentTag { get; set; }

    }
}