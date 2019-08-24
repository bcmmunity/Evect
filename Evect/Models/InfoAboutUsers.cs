using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evect.Models
{
    public class InfoAboutUsers
    {
        public int InfoAboutUsersId { get; set; }
        public int EventId { get; set; }
        public int AmountOfActivationsOfNetworking { get; set; } = 0;
        public int AmountOfRequestsOfContacts { get; set; } = 0;
        public int AmountOfRequestsOfMettings { get; set; } = 0;
    }
}
