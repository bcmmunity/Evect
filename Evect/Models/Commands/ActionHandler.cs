using System.Reflection;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class ActionHandler
    {
        private readonly CommandHandler _commandHadler = new CommandHandler();
        [UserAction(Actions.None)]
        public async void OnNone(Message message, TelegramBotClient client)
        {
            var commands = Bot.Commands;
            var chatId = message.Chat.Id;
            foreach (var methodInfo in commands)
            {
                var act = methodInfo.GetCustomAttribute<TelegramCommand>().StringCommand;
                if (act == "/start")
                {
                    methodInfo.Invoke(_commandHadler, new object[] { message, client});
                }
            }

            await client.SendTextMessageAsync(
                chatId,
                "Я не понимаю вас",
                ParseMode.Html);
        }
        
        
        [UserAction(Actions.Profile)]
        public async void OnProfile(Message message, TelegramBotClient client)
        {
            UserDB userDb = new UserDB();
            
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await userDb.GetUserByChatId(chatId);

            switch (text)
            {
                case "О мероприятии":
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
                    break;
                
                case "Присоединиться к мероприятию":
                    await client.SendTextMessageAsync(
                        chatId,
                        "Веедите ивент код",
                        ParseMode.Html);
                    userDb.ChangeUserAction(chatId, Actions.WaitingForEventCode);
                    break;
                
                case "Назад":
                    userDb.ResetAction(chatId);
                    await client.SendTextMessageAsync(
                        chatId,
                        "test",
                        ParseMode.Html);
                    break;
                
                default:
                    await client.SendTextMessageAsync(
                        chatId,
                        "чот не то",
                        ParseMode.Html);
                    break;
            }
            
        }
        
    }
}