using System;
using System.Collections.Generic;

namespace Evect.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string EventCode { get; set; }
        public string AdminCode { get; set; }
        public string TelegraphLink { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<UserEvent> UserEvents { get; set; }

    }
}