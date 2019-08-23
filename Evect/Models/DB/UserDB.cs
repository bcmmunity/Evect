using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public class UserDB : DB
    {

        public readonly ApplicationContext Context;
        
        public UserDB()
        {
            Context = Connect();
        }

        
        #region Add/Remove/Edit/Login/Logoff users
        
        public void AddUser(long tgId)//‰Ó·‡‚ÎÂÌËÂ ˛ÁÂ‡
        {
            Context.Users.Add(new User {TelegramId = tgId, IsAuthed = true});
            Context.SaveChanges();
        }
        

        public async void AddUserAsync(long tgId)
        {
            await Context.Users.AddAsync(new User { TelegramId = tgId, IsAuthed = true});
            await Context.SaveChangesAsync();
        }
        
        
    
        public async void UserLogin(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            user.IsAuthed = true;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }
        public async void AdminAuthorized(long tgId)//¿¬“Œ–»«¿÷»ﬂ ¿ƒÃ»ÕŒÃ
        {
            User user = await GetUserByChatId(tgId);
            user.IsAdminAuthorized = true;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }
        
        public async void UserLogoff(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            user.IsAuthed = false;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }

        public async void ResetAction(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            user.CurrentAction = Actions.None;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }
        
        
        #endregion
        
        public async Task<bool> IsUserExists(long tgId)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.TelegramId == tgId) != null;
        }
        
        
        public async Task<bool> IsUserExistsAndAuthed(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            return user != null && user.IsAuthed;
        }



        public async Task<User> GetUserByChatId(long tgId)
        {
            return await Context.Users
                .Include(u => u.UserEvents)
                .FirstOrDefaultAsync(u => u.TelegramId == tgId);
        }

        public async void ResetUserAction(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            user.CurrentAction = Actions.None;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }

        public async void ChangeUserAction(long tgId, Actions action)
        {
                 
            User user = await GetUserByChatId(tgId);
            user.CurrentAction = action;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }

       /* public void AddLog(string log)
        {
            Log logg = new Log();
            logg.Logss = log;
            Context.Logs.Add(logg);
            Context.SaveChanges();
        }*/
        //        public async void ChangeUserParams()





    }
}