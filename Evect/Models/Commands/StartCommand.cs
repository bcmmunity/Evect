using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class StartCommand : BaseCommand
    {
        public override string Name { get; } = "/start";
        public override async Task Execute(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB db = new UserDB();
            if (!await db.IsUserExists(chatId))
            {
                db.AddUser(chatId);
            }
            else if (!await db.IsUserExistsAndAuthed(chatId))
            {
                db.UserLogin(chatId);
                await client.SendTextMessageAsync(chatId, "Мы рады вас снова видеть", ParseMode.Html);
            }
            else
            {
                await client.SendTextMessageAsync(chatId, "Мы вас уже знаем", ParseMode.Html);
            }
            
            string[][] actions = { new[] { "Войти по ивент-коду" }, new[] {"Личный кабинет"} };
            await client.SendTextMessageAsync(chatId, "Чудненько "+"😇"+" Можем приступить", ParseMode.Markdown);
            await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по <b>ивент-коду</b> \n P.S.<b>Ивент-код</b> отправлен в письме регистрации",ParseMode.Html,replyMarkup:TelegramKeyboard.GetKeyboard(actions));
            
        }

        
    }
}