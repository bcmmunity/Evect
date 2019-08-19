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
        


        public async void AddUser(long tgId)
        {

            await Context.Users.AddAsync(new User() { TelegramId = tgId});
            await Context.SaveChangesAsync();
        }

        public async Task<bool> IsUserExists(long tgId)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.TelegramId == tgId) != null;
        }

        public async Task<User> GetUserByChatId(long tgId)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.TelegramId == tgId);
        }

        public async void ResetUserAction(long tgId)
        {
            User user = await GetUserByChatId(tgId);
            user.CurrentAction = Actions.None;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }
        
//        public async void ChangeUserParams()
        
        
        
        
        
    }
}