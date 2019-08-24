using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class CommandHandler
    {

        [TelegramCommand("/start")]
        public async void OnStart(ApplicationContext context, Message message, TelegramBotClient client)
        {
            UserDB.AddLog(context,"start0");
            long chatId = message.Chat.Id;
            UserDB.AddLog(context, "start1");
            if (!await UserDB.IsUserExists(context, chatId))
            {
                UserDB.AddLog(context, "start2");
                UserDB.AddUser(context, chatId);
                UserDB.AddLog(context, "start3");
            }
            else if (!await UserDB.IsUserExistsAndAuthed(context, chatId))
            {
                UserDB.UserLogin(context, chatId);
                await client.SendTextMessageAsync(chatId, "Мы рады вас снова видеть", ParseMode.Html);
            }
            else
            {
                await client.SendTextMessageAsync(chatId, "Мы вас уже знаем", ParseMode.Html);
            }

            string[][] actions = { new[] { "Войти по ивент-коду" }, new[] { "Личный кабинет" } };
            await client.SendTextMessageAsync(chatId, "Чудненько " + "😇" + " Можем приступить", ParseMode.Markdown);
            await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по **ивент-коду** \n P.S.**Ивент-код** отправлен в письме регистрации", ParseMode.Markdown, replyMarkup: TelegramKeyboard.GetKeyboard(actions));

        }


        [TelegramCommand("/stop")]
        public async void OnStop(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            
            User user = await UserDB.GetUserByChatId(context, chatId);

            if (await UserDB.IsUserExistsAndAuthed(context, chatId))
            {
                if (user.CurrentAction != Actions.DeleteOrNot)
                {
                    string[][] actions = { new[] { "Да" }, new[] { "Нет" } };
                    await client.SendTextMessageAsync(chatId, "**Сохранить** ваши данные или <b>полностью удалить</b>",
                        ParseMode.Markdown,
                        replyMarkup: TelegramKeyboard.GetKeyboard(actions, true));

                    UserDB.ChangeUserAction(context, chatId, Actions.DeleteOrNot);
                }

            }
        }

        [TelegramCommand("Личный кабинет")]
        public async void OnProfile(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            User user = await UserDB.GetUserByChatId(context, chatId);
            
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))//здесь мб сделать проверку на админский ли код
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Похоже мы не все о вас знаем. Как вас зовут? Попрошу имя и фамилию через пробел",
                    ParseMode.Html);
                UserDB.ChangeUserAction(context, chatId, Actions.WaitingForName);
                            
            } else if (string.IsNullOrEmpty(user.Email))
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Вот мы и познакомились, а теперь можно ваш адрес электронной почты?",
                    ParseMode.Html);
                UserDB.ChangeUserAction(context, chatId, Actions.WainingForEmail);
            }
            else
            {
                string[][] actions = { new[] { "О мероприятии", "Присоединиться к мероприятию" }, new[] {"Режим нетворкинга"}, new[] {"Записная книжка"}, new[] {"Все мероприятия"} };
                await client.SendTextMessageAsync(
                    chatId,
                    "Что нужно?",
                    ParseMode.Html,
                    replyMarkup: TelegramKeyboard.GetKeyboard(actions));
                    
                UserDB.ChangeUserAction(context, chatId, Actions.Profile);
            }
        }


        [TelegramCommand("Войти по ивент-коду")]
        public async void OnEnteringByCode(ApplicationContext context, Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Введите ивент-код",ParseMode.Html);

            UserDB.ChangeUserAction(context, chatId, Actions.WaitingForEventCode);
        }

     
      
       


    } 
}