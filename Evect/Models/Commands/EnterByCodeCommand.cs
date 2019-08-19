using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class EnterByCodeCommand : BaseCommand
    {
        public override string Name { get; } = "Войти по ивент-коду";
        public override async Task Execute(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB userDb = new UserDB();
            User user = await userDb.GetUserByChatId(chatId);
            user.CurrentAction = Actions.WaitingForEventCode;
            userDb.Context.Users.Update(user);
            await userDb.Context.SaveChangesAsync();
        }
        
    }
}