using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class ProfileCommand : BaseCommand
    {
        public override string Name { get; } = "Личный кабинет";
        public override async Task Execute(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB db = new UserDB();
            db.ChangeUserAction(chatId, Actions.Profile);
            string[][] actions = { new[] { "О мероприятии" }, new[] {"Присоединиться к мероприятию"} };
            await client.SendTextMessageAsync(chatId, "Что нужно?",ParseMode.Html,replyMarkup:TelegramKeyboard.GetKeyboard(actions));
        }
    }
}