using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class ActionHandler
    {
        [UserAction(Actions.Profile)]
        public async void OnProfile(Message message, TelegramBotClient client)
        {
            UserDB userDb = new UserDB();
            
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await userDb.GetUserByChatId(chatId);
            if (text == "О мероприятии")
            {
                bool isReg = user.CurrentEventId > 0;
                if (isReg)
                {
                    Event ev = userDb.Context.Events.Find(user.CurrentEventId);
                    await client.SendTextMessageAsync(
                        chatId,
                        $@"<b>Название: </b>{ev.Name}
<b>Описание: </b>{ev.Info}",
                        ParseMode.Html);
                }
                else
                {
                    await client.SendTextMessageAsync(
                        chatId,
                        $"Вы не присоединились ни к одному мероприятию",
                        ParseMode.Html);
                }
            }
            else if (text == "Присоединиться к мероприятию")
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Веедите ивент код",
                    ParseMode.Html);
                userDb.ChangeUserAction(chatId, Actions.WaitingForEventCode);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "чот не то",
                    ParseMode.Html);
            }
        }
        
    }
}