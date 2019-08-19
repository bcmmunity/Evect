using System;
using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class StopCommand : BaseCommand
    {
        public override string Name { get; } = "/stop";
        public override async Task Execute(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            
            UserDB db = new UserDB();
            User user = await db.GetUserByChatId(chatId);
            string text = message.Text;

            if (await db.IsUserExistsAndAuthed(chatId))
            {
                if (user.CurrentAction != Actions.DeleteOrNot)
                {
                    string[][] actions = { new[] { "Да" }, new[] {"Нет"} };
                    await client.SendTextMessageAsync(chatId, "<b>Сохранить</b> ваши данные или <b>полносью удалить</b>", 
                        ParseMode.Html,
                        replyMarkup: TelegramKeyboard.GetKeyboard(actions));
                    user.CurrentAction = Actions.DeleteOrNot;
                    db.Context.Users.Update(user);
                    await db.Context.SaveChangesAsync();
                }

            } 
            
        }
    }
}