using System.Collections.Generic;
using Evect.Models;
namespace Evect.Models
{
    public class User
    {
        public int UserId { get; set; }
        public long TelegramId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public bool IsAuthed { get; set; }
        public Actions CurrentAction { get; set; } = Actions.None;
        
        public int CurrentEventId { get; set; }
        public List<UserTag> UserTags { get; set; }
        public List<UserEvent> UserEvents { get; set; }

        public List<ContactsBook> Contacts { get; set; }
        
    }
}