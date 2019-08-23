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
            await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по <b>ивент-коду</b> \n P.S.<b>Ивент-код</b> отправлен в письме регистрации", ParseMode.Html, replyMarkup: TelegramKeyboard.GetKeyboard(actions));

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
                    await client.SendTextMessageAsync(chatId, "<b>Сохранить</b> ваши данные или <b>полносью удалить</b>",
                        ParseMode.Html,
                        replyMarkup: TelegramKeyboard.GetKeyboard(actions, true));

                    db.ChangeUserAction(chatId, Actions.DeleteOrNot);
                }

            }
        }

        [TelegramCommand("Личный кабинет")]
        public async void OnProfile(Message message, TelegramBotClient client)
        {
            long chatId = message.Chat.Id;
            UserDB db = new UserDB();
            db.ChangeUserAction(chatId, Actions.Profile);
            string[][] actions = { new[] { "О мероприятии" }, new[] {"Присоединиться к мероприятию"} };
            await client.SendTextMessageAsync(chatId, "Что нужно?",ParseMode.Html,replyMarkup:TelegramKeyboard.GetKeyboard(actions, true));
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