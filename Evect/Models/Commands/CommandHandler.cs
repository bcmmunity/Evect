using System.Text;
using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class CommandHandler
    {

        [TelegramCommand("/start")]
        public async Task OnStart(ApplicationContext context, Message message, TelegramBotClient client)
        {
         
            long chatId = message.Chat.Id;
            if (!await UserDB.IsUserExists(context, chatId))
            {
                 UserDB.AddUser(context, chatId, message.From.Username);
            }
            else if (!await UserDB.IsUserExistsAndAuthed(context, chatId))
            {
                await UserDB.UserLogin(context, chatId);
                await client.SendTextMessageAsync(chatId, "Мы рады вас снова видеть", ParseMode.Html);
            }
            else
            {
                await client.SendTextMessageAsync(chatId, "Мы вас уже знаем", ParseMode.Html);
            }

            TelegramKeyboard keyboard = new TelegramKeyboard();
            keyboard.AddRow("Войти по ивент-коду");
            keyboard.AddRow("Личный кабинет");

            await client.SendTextMessageAsync(chatId, "Чудненько " + "😇" + " Можем приступить", ParseMode.Markdown);
            await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по *ивент-коду* \n P.S.*Ивент-код* отправлен в письме регистрации", ParseMode.Markdown, replyMarkup: keyboard.Markup);

        }


        [TelegramCommand("/stop")]
        public async Task OnStop(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            
            User user = await UserDB.GetUserByChatId(context, chatId);

            if (await UserDB.IsUserExistsAndAuthed(context, chatId))
            {
                if (user.CurrentAction != Actions.DeleteOrNot)
                {
                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("Да");
                    keyboard.AddRow("Нет");
                    await client.SendTextMessageAsync(chatId, "**Сохранить** ваши данные или <b>полностью удалить</b>",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);

                    await UserDB.ChangeUserAction(context, chatId, Actions.DeleteOrNot);
                }

            }
        }

        [TelegramCommand("Личный кабинет")]
        public async Task OnProfile(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            User user = await UserDB.GetUserByChatId(context, chatId);
            StringBuilder builder = new StringBuilder();
            
            if (string.IsNullOrEmpty(user.Email))
            {
                builder.AppendLine(@"Я смотрю мы вас не знаем, войдите по ивент коду ");
                builder.AppendLine();
                builder.AppendLine(@"Введите ивент код");
                            
                await client.SendTextMessageAsync(
                    chatId,
                    builder.ToString(),
                    ParseMode.Markdown);
                await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForEventCode);
            }
            else
            {
                
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                keyboard.AddRow("Режим нетворкинга");
                keyboard.AddRow("Записная книжка");
                keyboard.AddRow("Все мероприятия");
                
                await client.SendTextMessageAsync(
                    chatId,
                    "Что нужно?",
                    ParseMode.Html,
                    replyMarkup: keyboard.Markup);
                    
                await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
            }
        }


        [TelegramCommand("Войти по ивент-коду")]
        public async Task OnEnteringByCode(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Пожалуйста, введите *ивент-код*",ParseMode.Markdown);

            await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForEventCode);
        }

     
      
       


    } 
}