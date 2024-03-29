using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public class UserDB : DB
    {

        public static void AddUser(ApplicationContext context, long tgId, string userName)//���������� �����
        {
            AddLog(context, "adduser0");
            context.Users.Add(new User {TelegramId = tgId, IsAuthed = true, TelegramUserName = userName});
            
            AddLog(context, "adduser1");
            context.SaveChanges();
            AddLog(context, "adduser2");
        }

        public static void AddMobileUser(ApplicationContext context, string email)
        {
            context.MobileUsers.Add(new MobileUser {Email = email});
            context.SaveChanges();
        }
        
        public static void AddMobileUser(ApplicationContext context, string email, string code)
        {
            User user = GetUserByEmail(context, email).GetAwaiter().GetResult();
            context.MobileUsers.Add(new MobileUser {Email = email, EmailCode = code, TelegramId = user?.TelegramId ?? default, ApiKey = Utils.GenerateNewCode(16)});
            context.SaveChanges();
        }
        

        public static async Task AddUserAsync(ApplicationContext context, long tgId)
        {
            await context.Users.AddAsync(new User { TelegramId = tgId, IsAuthed = true});
            context.SaveChanges();
        }

        public static async Task<bool> AddDeleteTag(ApplicationContext context, long tgId, int tagId)
        {
            User user = await GetUserByChatId(context, tgId);

            UserTag tg = user.UserTags.FirstOrDefault(e => e.TagId == tagId);
            
            bool ex = tg != null;

            if (ex)
            {
                context.UserTags.Remove(tg);
                context.SaveChanges();
                return false;
            }
            else
            {
                UserTag tag = new UserTag()
                {
                    UserId = user.UserId,
                    TagId = tagId
                };
                
                user.UserTags.Add(tag);
                context.Update(user);
                context.SaveChanges();
                return true;
            }

        }
        
        public static async Task<bool> AddDeleteSearchTag(ApplicationContext context, long tgId, int tagId)
        {
            User user = await GetUserByChatId(context, tgId);

            UserSearchingTag tg = user.SearchingUserTags.FirstOrDefault(e => e.TagId == tagId);
            
            bool ex = tg != null;

            if (ex)
            {
                context.UserSearchingTags.Remove(tg);
                context.SaveChanges();
                return false;
            }
            else
            {
                UserSearchingTag tag = new UserSearchingTag()
                {
                    UserId = user.UserId,
                    TagId = tagId
                };
                
                user.SearchingUserTags.Add(tag);
                context.Update(user);
                context.SaveChanges();
                return true;
            }

        }
    
        public static async Task UserLogin(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAuthed = true;
            context.Users.Update(user);
            context.SaveChanges();
        }
        public static async Task AdminAuthorized(ApplicationContext context, long tgId)//����������� �������
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAdminAuthorized = true;
            context.Users.Update(user);
            context.SaveChanges();
        }
        
        public static async Task UserLogoff(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAuthed = false;
            context.Users.Update(user);
            context.SaveChanges();
        }

        public static async Task ResetAction(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.CurrentAction = Actions.None;
            context.Users.Update(user);
            context.SaveChanges();
        }
        
        public static async Task<bool> IsUserExists(ApplicationContext context, long tgId)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.TelegramId == tgId) != null;
        }

        public static async Task<bool> IsMobileUserExists(ApplicationContext context, string email)
        {
            return await context.MobileUsers.FirstOrDefaultAsync(u => u.Email == email) != null;
        }
        
        
        
        public static async Task<bool> IsUserExistsAndAuthed(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            return user != null && user.IsAuthed;
        }



        public static async Task<User> GetUserByChatId(ApplicationContext context, long tgId)
        {
            return await context.Users
                .Include(u => u.UserEvents)
                .Include(u => u.Contacts)
                .Include(u => u.UserTags)
                .ThenInclude(u => u.Tag)
                .Include(u => u.SearchingUserTags)
                .ThenInclude(u => u.Tag)
                .FirstOrDefaultAsync(u => u.TelegramId == tgId);
        }
        
        public static async Task<User> GetUserByEmail(ApplicationContext context, string email)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        

        public static async Task<MobileUser> GetMobileUserByEmail(ApplicationContext context, string email)
        {
            return await context.MobileUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public static async Task ResetUserAction(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.CurrentAction = Actions.None;
            context.Users.Update(user);
            context.SaveChanges();
        }

        public static async Task ChangeUserAction(ApplicationContext context, long tgId, Actions action)
        {
                 
            User user = await GetUserByChatId(context, tgId);
            user.CurrentAction = action;
            context.Users.Update(user);
            context.SaveChanges();
        }

        public static async Task ChangeUserActionAsync(ApplicationContext context, long tgId, Actions action)
        {
            await Task.Run(() => ChangeUserAction(context, tgId, action));
        }

        /// <summary>
        ///  ��������� ������� ������������ � ������� ������� ����������� �����
        /// </summary>
        public static async Task<bool> CheckEmailInDB(ApplicationContext context, string email)
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }
        
        public static void AddSurvey(ApplicationContext context,int type,string message,long chatId)
        {
            User user = context.Users.FirstOrDefault(n => n.TelegramId == chatId);
            Question question = new Question();
            question.EventId = user.CurrentEventId;
            question.Questions = message;
            question.Type = type;
            context.Questions.Add(question);
            context.SaveChanges();
        }
        public static int GetQuestionId(ApplicationContext context,string message)
        {
            Question question = context.Questions.FirstOrDefault(n => n.Questions == message);
            return question.QuestionId;
        }
        
        
        public static void AddLog(ApplicationContext context,string log)
        {
            Log logg = new Log();
            logg.Logss = log;
            context.Logs.Add(logg);
            context.SaveChanges();
        }
        //        public static async Task ChangeUserParams()





    }
}