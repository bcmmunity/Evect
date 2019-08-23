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
        
        public async Task<bool> IsAdminCode(string code)
        {
            return await Context.Events.FirstOrDefaultAsync(ev => ev.AdminCode == code) != null;
        }
        public async Task<string> GetInformationAboutEvent(long tgId)
        {
            User user =await Context.Users.FirstOrDefaultAsync(n=>n.TelegramId==tgId);
            int idOfCurrentEvent = user.CurrentEventId;
            Event currentEvent = await Context.Events.FirstOrDefaultAsync(ev => ev.EventId == idOfCurrentEvent);
            string temp = "";
            temp = temp + currentEvent.Info;
            return temp;
        }
        public async void AddInformationAboutEvent(long chatid,string information)
        {
            User user = await Context.Users.FirstOrDefaultAsync(n => n.TelegramId == chatid);
            int eventId = user.CurrentEventId;
            Event currentEvent = await Context.Events.FirstOrDefaultAsync(n => n.EventId == eventId);
            currentEvent.Info = information;
            Context.SaveChanges();
            
        }
        
    }
}