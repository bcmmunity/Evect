using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public class UserDB : DB
    {

        public static void AddUser(ApplicationContext context, long tgId)//добавление юзера
        {
            AddLog(context, "adduser0");
            context.Users.Add(new User {TelegramId = tgId, IsAuthed = true});
            AddLog(context, "adduser1");
            context.SaveChanges();
            AddLog(context, "adduser2");
        }
        

        public static async void AddUserAsync(ApplicationContext context, long tgId)
        {
            await context.Users.AddAsync(new User { TelegramId = tgId, IsAuthed = true});
            context.SaveChanges();
        }
        
        
    
        public static async void UserLogin(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAuthed = true;
            context.Users.Update(user);
            context.SaveChanges();
        }
        public static async void AdminAuthorized(ApplicationContext context, long tgId)//АВТОРИЗАЦИЯ АДМИНОМ
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAdminAuthorized = true;
            context.Users.Update(user);
            context.SaveChanges();
        }
        
        public static async void UserLogoff(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.IsAuthed = false;
            context.Users.Update(user);
            context.SaveChanges();
        }

        public static async void ResetAction(ApplicationContext context, long tgId)
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
        
        
        public static async Task<bool> IsUserExistsAndAuthed(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            return user != null && user.IsAuthed;
        }



        public static async Task<User> GetUserByChatId(ApplicationContext context, long tgId)
        {
            return await context.Users
                .Include(u => u.UserEvents)
                .FirstOrDefaultAsync(u => u.TelegramId == tgId);
        }

        public static async void ResetUserAction(ApplicationContext context, long tgId)
        {
            User user = await GetUserByChatId(context, tgId);
            user.CurrentAction = Actions.None;
            context.Users.Update(user);
            context.SaveChanges();
        }

        public static async void ChangeUserAction(ApplicationContext context, long tgId, Actions action)
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
        ///  Проверяет наличие пользователя с введным адресом электронной почти
        /// </summary>
        public static async Task<bool> CheckEmailInDB(ApplicationContext context, string email)
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }
        
        
        
        
        public static void AddLog(ApplicationContext context,string log)
        {
            Log logg = new Log();
            logg.Logss = log;
            context.Logs.Add(logg);
            context.SaveChanges();
        }
        //        public static async void ChangeUserParams()





    }
}