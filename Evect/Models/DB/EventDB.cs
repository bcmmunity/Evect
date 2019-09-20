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
                    temp = temp + item.Info+"\n";
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
        public async Task<List<long>> GetAllParticipantsOfEvent(int IdOfEvent)
        {
            int EventId = IdOfEvent;
            List<User> AllUsers = await Context.Users.ToListAsync();
            List<long> UsersToSend = new List<long>();
            foreach (var item in AllUsers)
            {
                if (item.CurrentEventId == EventId)
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
                        return info.AmountCompletedMeetings.ToString();
                    }
            }
            return "Бот не знает такой команды";
        }
        public async Task<string> GetInfrormationAboutUsers(int idOfEvent, string message)
        {
            Event eventt = await Context.Events.FirstOrDefaultAsync(n => n.EventId == idOfEvent);
            InfoAboutUsers info = await Context.InfoAboutUsers.FirstOrDefaultAsync(m => m.EventId == idOfEvent);
            switch (message)
            {
                case "Количество пользователей":
                    {
                        List<long> participants = await GetAllParticipantsOfEvent(idOfEvent);
                        string t = participants.Count.ToString();
                        return t;
                    }
                case "Количество использования режим общения":
                    {
                        return info.AmountOfActivationsOfNetworking.ToString();
                    }
                case "Сколько встреч согласовано":
                    {
                        return info.AmountCompletedMeetings.ToString();
                    }
                case "Сколько запрошено контактов":
                    {
                        return info.AmountOfRequestsOfContacts.ToString();
                    }
                case "Среднее число контактов":
                    {
                        return info.AverageAmountOfContact.ToString();
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
                {   case "Количество использования режим общения":
                    {
                        info.AmountOfActivationsOfNetworking++;
                    }
                        break;
                    case "Сколько встреч согласовано":
                        {
                            info.AmountCompletedMeetings++;
                        }
                        break;
                    case "Сколько запрошено контактов":
                        {
                            info.AmountOfRequestsOfContacts++;
                        }
                        break;
                    case "Среднее число контактов":
                        {
                        
                        }
                        break;
                 
                }
            }
            
        }
        public List<int> GetIdOfQuestions(ApplicationContext context,int eventId)
        {
            List<int> Questions = new List<int>();
            List<Question> allQuestions = context.Questions.ToList();
            foreach(var question in allQuestions)
            {
                if(question.EventId==eventId)
                {
                   Questions.Add(question.QuestionId);
                }
            }
            return Questions;
        }
        public int GetCountOfRespondents(ApplicationContext context,int idOfQuestion)
        {
            List<Answer> answers = context.Answers.ToList();
            int count = 0;
            foreach(var answer in answers)
            {
                if (answer.QuestionId == idOfQuestion)
                    count++;
            }
            return count;
        }
        public string GetTypeOfQuestion(ApplicationContext context, int idOfQuestion)
        {
            Question question = context.Questions.FirstOrDefault(n => n.QuestionId == idOfQuestion);
           int type=question.Type;
            string str = "";
            if (type == 0) str = "Опрос с развернутой частью";
            else str = "Опрос с оценкой";
            return str;
        }
        public List<string> GetTags(ApplicationContext context,int EventId,string typeOfTags)
        {
            List<Tag> tags = context.Tags.ToList();
            List<string> tagsForReturning = new List<string>();

            if(typeOfTags=="Parent")
            {
                foreach(var tag in tags)
                {
                    if (tag.Level == 1)
                        tagsForReturning.Add(tag.Name);
                }
            }
            else if(typeOfTags=="Child")
            {
                foreach(var tag in tags)
                {
                    if (tag.Level == 2)
                        tagsForReturning.Add(tag.Name);
                }
            }
            return tagsForReturning;
        }
    }
}