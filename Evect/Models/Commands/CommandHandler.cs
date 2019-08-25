using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class CommandHandler
    {

        [TelegramCommand("/start")]
        public async void OnStart(Message message, TelegramBotClient client)
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

            string[][] actions = { new[] { "Войти по ивент-коду" }, new[] { "Личный кабинет" } };
            await client.SendTextMessageAsync(chatId, "Чудненько " + "😇" + " Можем приступить", ParseMode.Markdown);
            await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по **ивент-коду** \n P.S.**Ивент-код** отправлен в письме регистрации", ParseMode.Markdown, replyMarkup: TelegramKeyboard.GetKeyboard(actions));

        }


        [TelegramCommand("/stop")]
        public async void OnStop(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;

            UserDB db = new UserDB();
            User user = await db.GetUserByChatId(chatId);

            if (await db.IsUserExistsAndAuthed(chatId))
            {
                if (user.CurrentAction != Actions.DeleteOrNot)
                {
                    string[][] actions = { new[] { "Да" }, new[] { "Нет" } };
                    await client.SendTextMessageAsync(chatId, "**Сохранить** ваши данные или <b>полносью удалить</b>",
                        ParseMode.Markdown,
                        replyMarkup: TelegramKeyboard.GetKeyboard(actions, true));

                    db.ChangeUserAction(chatId, Actions.DeleteOrNot);
                }

            }
        }

        [TelegramCommand("Личный кабинет")]
        public async void OnProfile(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB userDb = new UserDB();
            User user = await userDb.GetUserByChatId(chatId);
            
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))//здесь мб сделать проверку на админский ли код
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Похоже мы не все о вас знаем. Как вас зовут? Попрошу имя и фамилию через пробел",
                    ParseMode.Html);
                userDb.ChangeUserAction(chatId, Actions.WaitingForName);
                            
            } else if (string.IsNullOrEmpty(user.Email))
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Вот мы и познакомились, а теперь можно ваш адрес электронной почты?",
                    ParseMode.Html);
                userDb.ChangeUserAction(chatId, Actions.WainingForEmail);
            }
            else
            {
                string[][] actions = { new[] { "О мероприятии", "Присоединиться к мероприятию" }, new[] {"Режим нетворкинга"}, new[] {"Записная книжка"}, new[] {"Все мероприятия"} };
                await client.SendTextMessageAsync(
                    chatId,
                    "Что нужно?",
                    ParseMode.Html,
                    replyMarkup: TelegramKeyboard.GetKeyboard(actions));
                    
                userDb.ChangeUserAction(chatId, Actions.Profile);
            }
        }


        [TelegramCommand("Войти по ивент-коду")]
        public async void OnEnteringByCode(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB userDb = new UserDB();
            await client.SendTextMessageAsync(chatId, "Введите ивент-код",ParseMode.Html);

            userDb.ChangeUserAction(chatId, Actions.WaitingForEventCode);
        }

     
      
       


    } 
}