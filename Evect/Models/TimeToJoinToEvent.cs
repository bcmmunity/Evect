using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evect.Models
{
    public class TimeToJoinToEvent
    {
        public int TimeToJoinToEventId { get; set; }
        public long TelegramId { get; set; }
        public int EventId { get; set; }
        public DateTime time { get; set; }
    }
}
