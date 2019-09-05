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

        public async Task<List<Event>> GetUserEvents(long chatId)
        {
            User user = await UserDB.GetUserByChatId(Context, chatId);
            List<Event> events = new List<Event>();

            if (user.CurrentEventId > 0)
            {
                events.Add(Context.Events.Find(user.CurrentEventId));
            }
            
            
            foreach (var ue in user.UserEvents)
            {
                if (ue.EventId == user.CurrentEventId) continue;
                events.Add(await GetEventByUserEvent(ue));
            }

            return events;
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
                    if(item.Info!=null)
                    temp = temp + item.Info;
                    if (item.TelegraphLink != null)
                        temp = temp + item.TelegraphLink;
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
        public async Task<List<long>> GetAllParticipantsOfEvent(long chatId)
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
        public async Task<string> GetInfrormationAboutUsers(long chatId,string message)
        {
            User user = await Context.Users.FirstOrDefaultAsync(n => n.TelegramId == chatId);
            int eventId = user.CurrentEventId;
            Event eventt = await Context.Events.FirstOrDefaultAsync(n => n.EventId == eventId);
            InfoAboutUsers info = await Context.InfoAboutUsers.FirstOrDefaultAsync(m => m.EventId == eventId);
            switch (message)
            {
                case "Количество пользователей":
                    {
                       List<long>participants= await GetAllParticipantsOfEvent(chatId);
                        string t = participants.Count.ToString();
                        return t;
                    }
                case "Количество активаций режима общения":
                    {
                       return info.AmountOfActivationsOfNetworking.ToString() ;
                    }
                case "Число запросов контактов":
                    {
                        return info.AmountOfRequestsOfContacts.ToString();
                    }
                case "Число запросов встреч":
                    {
                        return  info.AmountOfRequestsOfMettings.ToString();
                    }
            }
            return "Бот не знает такой команды";
        }
        public async void AddInfoAboutUsers(long chatId,string type)
        {//добавить в info о пользователях eventId
            User user=await Context.Users.FirstOrDefaultAsync(n => n.TelegramId == chatId);
            int eventId = user.CurrentEventId;
            Event eventt = await Context.Events.FirstOrDefaultAsync(n => n.EventId == eventId);
            InfoAboutUsers info = await Context.InfoAboutUsers.FirstOrDefaultAsync(m => m.EventId == eventId);
            if (info != null)
            {
                switch (type)
                {   case "Количество активаций режима общения":
                    {
                        info.AmountOfActivationsOfNetworking++;
                    }
                        break;
                    case "Число запросов контактов":
                    {
                        info.AmountOfRequestsOfContacts++;
                    }
                        break;
                    case "Число запросов встреч":
                    {
                        info.AmountOfRequestsOfMettings++;
                    }
                        break;
                }
            }
            
        }
        
    }
}