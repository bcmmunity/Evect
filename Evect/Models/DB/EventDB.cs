using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public class EventDB : DB
    {

        public readonly ApplicationContext Context;
        
        public EventDB()
        {
            Context = Connect();
        }

        public async Task<bool> IsEventCodeValid(string code)
        {
            return await Context.Events.FirstOrDefaultAsync(ev => ev.EventCode == code || ev.AdminCode == code) != null;
        }

        public async Task<Event> GetEventByUserEvent(UserEvent ue)
        {
            return await Context.Events.FirstOrDefaultAsync(e => e.EventId == ue.EventId);
        }
        
    }
}