using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
      public string GetInfoAboutTheEvent(long tgId)
        {
            List<User> users = Context.Users.ToList();
            int EventId = 0;
            foreach(var item in users)
            {
                if(item.TelegramId==tgId)
                {
                    EventId = item.CurrentEventId;
                }
            }
            List<Event> events = Context.Events.ToList();
            string temp = "";
            foreach(var item in events)
            {
                if(item.EventId==EventId)
                {
                    temp = temp + item.Info;
                }
            }
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
        public async Task<List<long>> GetAllParticipants(long chatId,string message)
        {
            User user = await Context.Users.FirstOrDefaultAsync(n => n.TelegramId == chatId);
            int EventId = user.CurrentEventId;
            List<User> AllUsers =await Context.Users.ToListAsync();
            List<long> UsersToSend = new List<long>();
            foreach(var item in AllUsers)
            {
                if(item.CurrentEventId==EventId)
                {
                    UsersToSend.Add(item.TelegramId);
                }
            }
            return UsersToSend;
            
        }
        
    }
}