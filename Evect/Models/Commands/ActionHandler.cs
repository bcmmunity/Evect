using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public class ActionHandler
    {
        private readonly CommandHandler _commandHadler = new CommandHandler();

        [UserAction(Actions.None)]
        public async Task OnNone(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var commands = Bot.Commands;
            var chatId = message.Chat.Id;
            var text = message.Text;

            foreach (var pair in commands)
            {
                if (pair.Value == text)
                {
                    await pair.Key(context, message, client);
                    return;
                }
            }


            await client.SendTextMessageAsync(
                chatId,
                "Я не понимаю вас",
                ParseMode.Markdown);
        }


        [UserAction(Actions.WaitingForEventCode)]
        public async Task OnWaitingEventCode(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            EventDB eventDb = new EventDB();


            User user = await UserDB.GetUserByChatId(context, chatId);


            bool isValid = await eventDb.IsEventCodeValid(text);
            if (isValid)
            {
                Event ev = await context.Events.FirstOrDefaultAsync(e =>
                    e.EventCode == text || e.AdminCode == text);

                bool have = user.UserEvents.FirstOrDefault(ue => ue.EventId == ev.EventId) != null;

                bool isAdminCode = await eventDb.IsAdminCode(text);


                if (isAdminCode)
                {
                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("Об ивенте");
                    keyboard.AddRow("Инвормация о пользователях");
                    keyboard.AddRow("Создать опрос");
                    keyboard.AddRow("Создать оповещение");

                    await UserDB.AdminAuthorized(context, chatId);
                    await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                    if (!have)
                    {
                        UserEvent userEvent = new UserEvent() {UserId = user.UserId, EventId = ev.EventId};
                        user.UserEvents.Add(userEvent);
                        user.CurrentEventId = ev.EventId;
                        //почему не работает,когда это раскоменчено?
                        context.Users.Update(user);
                        context.SaveChanges();
                    }

                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine($"Включён режим организатора на мероприятии \"{ev.Name}\"");
//                    builder.AppendLine(@"😇" + "\n" + "Вам доступен расширенный функционал:");
//                    builder.AppendLine("0");
//                    builder.AppendLine("<b>Об ивенте</b>- внести изменение в информацию о мероприятии");
//                    builder.AppendLine("1");
//                    builder.AppendLine("Можно получить <b>информацию по всем участникам</b");
//                    builder.AppendLine("2" );
//                    builder.AppendLine("<b>Создать опрос</b>- опрос рассылается всем участникам, тип опроса- оценка от 1 до 5");
//                    builder.AppendLine("3");
//                    builder.AppendLine("<b>Создать оповещение</b>- сообщение отправляется всем участникам");

                    await client.SendTextMessageAsync(chatId,
                        builder.ToString(),
                        ParseMode.Html, replyMarkup: keyboard.Markup);
                }
                else
                {
                    if (have)
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            "Вы уже присоединились к этому мероприятию",
                            ParseMode.Markdown);


                        await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                        TelegramKeyboard keyboard = new TelegramKeyboard();
                        keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                        keyboard.AddRow("Режим нетворкинга");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Все мероприятия");
                        await client.SendTextMessageAsync(
                            chatId,
                            "Что нужно?",
                            ParseMode.Markdown,
                            replyMarkup: keyboard.Markup);
                    }
                    else
                    {
                        UserEvent userEvent = new UserEvent()
                        {
                            UserId = user.UserId,
                            EventId = ev.EventId
                        };

                        user.UserEvents.Add(userEvent);
                        user.CurrentEventId = ev.EventId;
                        context.Users.Update(user);
                        context.SaveChanges();


                        await client.SendTextMessageAsync(
                            chatId,
                            $"Вы подключаетесь к *{ev.Name}*",
                            ParseMode.Markdown);

                        StringBuilder builder = new StringBuilder();

                        if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName)
                        ) //здесь мб сделать проверку на админский ли код
                        {
                            await client.SendTextMessageAsync(
                                chatId,
                                "А теперь познакомимся. Как вас зовут?",
                                ParseMode.Markdown);
                            await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForName);
                        }
                        else if (string.IsNullOrEmpty(user.Email))
                        {
                            builder.AppendLine(@"Вот мы и познакомились✌️");
                            builder.AppendLine("А теперь введите адрес *электронной почты*");
                            builder.AppendLine();
                            builder.AppendLine(
                                "🤖 Он нужен для создания вашего *личного кабинета*, он упростит вход в случае отключения бота, а также это необходимо для проверки регистрации");

                            await client.SendTextMessageAsync(
                                chatId,
                                builder.ToString(),
                                ParseMode.Markdown);
                            await UserDB.ChangeUserAction(context, chatId, Actions.WainingForEmail);
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
                                ParseMode.Markdown,
                                replyMarkup: keyboard.Markup);

                            await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                        }
                    }
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    $"Неправильный код(",
                    ParseMode.Markdown);
            }
        }

        #region AdminModeAndAdminActions

        [UserAction(Actions.AdminMode)]
        public async Task AdminMode(ApplicationContext context, Message message, TelegramBotClient client)
        {
            if (message.Text == "Об ивенте")
            {
                EventDB eventDb = new EventDB();
                long chatId = message.Chat.Id;
                User user = await UserDB.GetUserByChatId(context, chatId);
                if (!user.IsAdminAuthorized)
                {
                    await client.SendTextMessageAsync(chatId, "Я не знаю такой команды пока");
                }
                else
                {
                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("Добавить новую статью");
                    keyboard.AddRow("Редактировать");
                    keyboard.AddRow("Назад");
                    await UserDB.ChangeUserAction(context, chatId, Actions.GetInformationAboutTheEvent);
                    string info = eventDb.GetInfoAboutTheEvent(chatId);
                    await client.SendTextMessageAsync(chatId, info, replyMarkup: keyboard.Markup);
                }
            }
            else if (message.Text == "Информация о пользователях")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                var chatId = message.Chat.Id;
                keyboard.AddRow("Количество пользователей");
                keyboard.AddRow("Количество активаций режима общения");
                keyboard.AddRow("Число запросов контактов");
                keyboard.AddRow("Число запросов встреч");
                keyboard.AddRow("Назад");
                await UserDB.ChangeUserAction(context, chatId, Actions.InformationAboutUsers);
                await client.SendTextMessageAsync(chatId, "Выберите интересующий Вас пункт",
                    replyMarkup: keyboard.Markup);
                // await client.SendTextMessageAsync(chatId, "Эти функции очень скоро будут доступны" + "😅".ToString(), ParseMode.Markdown);
            }
            else if (message.Text == "Создать оповещение")
            {
                //EventDB eventDB = new EventDB();
                long chatId = message.Chat.Id;
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateNotification);
                await client.SendTextMessageAsync(chatId,
                    "Отправьте сообщение,оно будет разослано всем участникам мероприятия",
                    replyMarkup: keyboard.Markup);
            }

            if(message.Text=="Создать опрос")
            {
                long chatId = message.Chat.Id;
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                keyboard.AddRow("Опрос с развернутой обратной связью");
                keyboard.AddRow("Опрос с оценкой");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Выберите тип опроса", replyMarkup: keyboard.Markup);
            }
        }

        [UserAction(Actions.GetInformationAboutTheEvent)]
        public async Task InformationAboutTheEvent(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            TelegramKeyboard backKeyboard = new TelegramKeyboard();
            backKeyboard.AddRow("Назад");

            if (text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Об ивенте");
                keyboard.AddRow("Инвормация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");

                await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                await client.SendTextMessageAsync(chatId, "Я вернулся в главное меню", replyMarkup: keyboard.Markup);
            }
            else if (text == "Редактировать")
            {
                await UserDB.ChangeUserAction(context, chatId, Actions.EditInformationAboutEvent);
                EventDB eventDb = new EventDB();
                string info = eventDb.GetInfoAboutTheEvent(chatId);

                // await client.SendTextMessageAsync(chatId, "Вы можете отредактировать информацию о мероприятии", replyMarkup: backKeybaord.Markup);
            }
            else if (text == "Добавить новую статью")
            {
                await UserDB.ChangeUserAction(context, chatId, Actions.AddNewInformationAboutEvent);
                string[][] back = {new[] {"Назад"}};
                await client.SendTextMessageAsync(chatId, "Отправьте статью обычным сообщением",
                    replyMarkup: backKeyboard.Markup);
            }
            else
            {
                EventDB eventDb = new EventDB();
                eventDb.AddInformationAboutEvent(chatId, text);
                await client.SendTextMessageAsync(chatId, "Данные о мероприятии успешно сохранены");
            }
        }

        [UserAction(Actions.AddNewInformationAboutEvent)] //"Добавить новую статью"
        public async Task AddInfoAboutEvent(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            EventDB eventDb = new EventDB();

            TelegramKeyboard keyboard = new TelegramKeyboard();
            keyboard.AddRow("Добавить новую статью");
            keyboard.AddRow("Редактировать");
            keyboard.AddRow("Назад");

            await UserDB.ChangeUserAction(context, chatId, Actions.GetInformationAboutTheEvent);
            if (message.Text == "Назад")
            {
                await client.SendTextMessageAsync(chatId, "Я вернулся назад", replyMarkup: keyboard.Markup);
            }
            else
            {
                eventDb.AddInformationAboutEvent(chatId, text);
                await client.SendTextMessageAsync(chatId, "Данные о мероприятии успешно добавлены",
                    replyMarkup: keyboard.Markup);
            }
        }

        [UserAction(Actions.CreateNotification)] //создать оповещение
        public async Task SendNotification(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            if (message.Text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Об ивенте");
                keyboard.AddRow("Инвормация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");

                await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);

                await client.SendTextMessageAsync(chatId, "Я вернулся в главное меню", replyMarkup: keyboard.Markup);
            }
            else
            {
                EventDB eventDb = new EventDB();
                List<long> usersToSend = await eventDb.GetAllParticipantsOfEvent(chatId);
                foreach (var item in usersToSend)
                {
                    await client.SendTextMessageAsync(item, text,
                        ParseMode.Markdown); //проверить,работает ли с форматированием?
                }

                await client.SendTextMessageAsync(chatId,
                    "Ваше сообщение успешно разослано всем участникам мероприятия");
            }
        }

        [UserAction(Actions.InformationAboutUsers)]
        public async Task GetInformationAboutUsers(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            EventDB eventDb = new EventDB();
            switch (message.Text)
            {
                case "Назад":
                    {
                        TelegramKeyboard keyboard = new TelegramKeyboard();
                        keyboard.AddRow("Об ивенте");
                        keyboard.AddRow("Инвормация о пользователях");
                        keyboard.AddRow("Создать опрос");
                        keyboard.AddRow("Создать оповещение");
                        await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                        await client.SendTextMessageAsync(chatId, "Я вернулся назад", replyMarkup: keyboard.Markup);
                    }
                    break;
                case "Количество пользователей":
                    {
                        string amountOfUsers = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfUsers);
                    }
                    break;
                case "Количество активаций режима общения":
                    {
                        string amountOfActivationOfNetworkingMode =
                            await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfActivationOfNetworkingMode);
                    }
                    break;
                case "Число запросов контактов":
                    {
                        string amountOfRequestsOfContacts = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfRequestsOfContacts);
                    }
                    break;
                case "Число запросов встреч":
                    {
                        string amountOfRequestsOfMeetings = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfRequestsOfMeetings);
                        break;
                    }
            }
        }
        [UserAction(Actions.CreateSurvey)]
        public async Task CreatingSurvey(ApplicationContext context,Message message,TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            if(text== "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Об ивенте");
                keyboard.AddRow("Инвормация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");
              await  UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                await client.SendTextMessageAsync(chatId, "Вы вернулись в главное меню",replyMarkup:keyboard.Markup);
            }
            else if (text== "Опрос с развернутой обратной связью")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
              await UserDB.ChangeUserAction(context, chatId, Actions.SurveyWithMessage);
                await client.SendTextMessageAsync(chatId, "Напишите название опроса", replyMarkup: keyboard.Markup);

            }
            else if (text== "Опрос с оценкой")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                await UserDB.ChangeUserAction(context, chatId, Actions.SurveyWithMessage);
                await client.SendTextMessageAsync(chatId, "Напишите название опроса", replyMarkup: keyboard.Markup);
            }

        }
        
        [UserAction(Actions.SurveyWithMessage)]
        public async Task OnCreatingWithMessage(ApplicationContext context,Message message,TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            if (message.Text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                keyboard.AddRow("Опрос с развернутой обратной связью");
                keyboard.AddRow("Опрос с оценкой");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Вы вернулись к выбору типа опроса", replyMarkup: keyboard.Markup);
            }
            else
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
               UserDB.AddSurvey(context, "Название опроса", message.Text,chatId);
                await client.SendTextMessageAsync(chatId, "Напишите сам вопрос,он будет отправлен всем участникам мероприятия",replyMarkup:keyboard.Markup);
               await UserDB.ChangeUserAction(context,chatId, Actions.QuestionForSurveyWithMessage);
            }
        }
        [UserAction(Actions.QuestionForSurveyWithMessage)]
        public async void AQuestionForSurveyWithMessage(ApplicationContext context,Message message,TelegramBotClient client)
        {
            TelegramKeyboard keyboard = new TelegramKeyboard();
            keyboard.AddRow("Назад");
            keyboard.AddRow("Опрос с развернутой обратной связью");
            keyboard.AddRow("Опрос с оценкой");
            var chatId = message.Chat.Id;
            if (message.Text=="Назад")
            {
                await UserDB.ChangeUserAction(context,chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Я вернулся к выбору типа опроса", replyMarkup: keyboard.Markup);
            }
            else
            {
                UserDB.AddSurvey(context, "Вопрос опроса",message.Text,chatId);
                EventDB eventDb = new EventDB();
               List<long> participants=await eventDb.GetAllParticipantsOfEvent(chatId);
                string text = message.Text;
                int QuestionId = UserDB.GetQuestionId(context,message.Text);
                TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();
                inlineKeyboard.AddTextRow("Ответить").AddCallbackRow("9-"+QuestionId.ToString());
                
                foreach (var participant in participants)
                {
                    await client.SendTextMessageAsync(participant, text,replyMarkup: inlineKeyboard.Markup);
                }
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Ваш вопрос успешно отослан всем участникам мероприятия", replyMarkup: keyboard.Markup);
            }
        }
      //  [InlineCallback("Ответить","9-")]//ТАК ЭТО ДЕЛАЕТСЯ?????
      //  [UserAction(Actions.SurveyWithMarks)]

        #endregion

        [UserAction(Actions.WaitingForName)]
        public async Task OnWaitingForName(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            var names = text.Split(' ');
            if (names.Length == 2)
            {
                user.FirstName = names[0];
                user.LastName = names[1];
                context.Update(user);
                context.SaveChanges();
                StringBuilder builder = new StringBuilder();

                if (string.IsNullOrEmpty(user.Email))
                {
                    builder.AppendLine(@"Вот мы и познакомились✌️");
                    builder.AppendLine("А теперь введите адрес *электронной почты*");
                    builder.AppendLine();
                    builder.AppendLine(
                        "🤖 Он нужен для создания вашего *личного кабинета*, он упростит вход в случае отключения бота, а также это необходимо для проверки регистрации");
                    await client.SendTextMessageAsync(
                        chatId,
                        builder.ToString(),
                        ParseMode.Markdown);
                    await UserDB.ChangeUserAction(context, chatId, Actions.WainingForEmail);
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Введите пожалуйста корректно имя и фамилию",
                    ParseMode.Markdown);
            }
        }

        [UserAction(Actions.WainingForEmail)]
        public async Task OnWaitingForEmail(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            if (Utils.IsEmailValid(text))
            {
                if (await UserDB.CheckEmailInDB(context, text))
                {
                    await client.SendTextMessageAsync(
                        chatId,
                        "Пользователь с этой почтой ранее использовал другой аккаунт телеграм. На эту почту отпарвлен код идентификации. Пожалуйста введите код",
                        ParseMode.Markdown);

                    string code = Utils.GenerateRandomCode();
                    await Utils.SendEmailAsync(text, "Потверждение почты", $"Ваш кода для потверждения почты: {code}");
                    await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForValidationCode);
                    context.Validations.Add(new UserValidation
                    {
                        UserTelegramId = chatId,
                        Code = code,
                        Email = text
                    });
                    context.SaveChanges();
                }
                else
                {
                    user.Email = text;
                    context.Update(user);
                    context.SaveChanges();

                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                    keyboard.AddRow("Режим нетворкинга");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Все мероприятия");

                    await client.SendTextMessageAsync(
                        chatId,
                        "Прекрасно, вам доступен весь мой функционал",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Введите, пожалуйста, почту корректно",
                    ParseMode.Markdown);
            }
        }

        [UserAction(Actions.WaitingForValidationCode)]
        public async Task OnWaitingForValidationCode(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var text = message.Text;
            var chatId = message.Chat.Id;


            User user = await UserDB.GetUserByChatId(context, chatId);
            UserValidation val = context.Validations.FirstOrDefault(v => v.UserTelegramId == chatId);
            if (val != null)
            {
                if (val.Code == text)
                {
                    User old = context.Users.FirstOrDefault(u => u.Email == val.Email);
                    if (old != null)
                    {
                        context.Users.Remove(old);
                        context.Validations.Remove(val);
                    }

                    user.Email = val.Email;
                    context.Users.Update(user);

                    context.SaveChanges();

                    await client.SendTextMessageAsync(
                        chatId,
                        "Почта успешно потверждена",
                        ParseMode.Markdown);


                    TelegramKeyboard keyboard = new TelegramKeyboard();
                    keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                    keyboard.AddRow("Режим нетворкинга");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Все мероприятия");
                    await client.SendTextMessageAsync(
                        chatId,
                        "Что нужно?",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);

                    await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                }
                else
                {
                    await client.SendTextMessageAsync(
                        chatId,
                        "Введите правильный код",
                        ParseMode.Markdown);
                }
            }
        }


        [UserAction(Actions.DeleteOrNot)]
        public async Task OnDeleteOrNot(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            if (text == "Да")
            {
                await UserDB.UserLogoff(context, chatId);
                await UserDB.ResetAction(context, chatId);
                await client.SendTextMessageAsync(
                    chatId,
                    "Вы успешно прекратили пользоваться evectbot, для того чтобы начать заново напишите <em>/start</em>",
                    ParseMode.Markdown);
            }
            else if (text == "Нет")
            {
                context.Users.Remove(user);
                context.SaveChanges();
                await client.SendTextMessageAsync(
                    chatId,
                    "Вся информация удалена, для того чтобы начать заново напишите <em>/start</em>",
                    ParseMode.Markdown);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Да/Нет",
                    ParseMode.Markdown);
            }
        }


        [UserAction(Actions.Profile)]
        public async Task OnProfile(ApplicationContext context, Message message, TelegramBotClient client)
        {
            EventDB eventDb = new EventDB();
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);
            StringBuilder builder = new StringBuilder();

            TelegramKeyboard keyboard;
            switch (text)
            {
                case "О мероприятии":
                    bool isReg = user.CurrentEventId > 0;
                    if (isReg)
                    {
                        Event ev = context.Events.Find(user.CurrentEventId);
                        string linkType = ev.TelegraphLink.Contains("telegra.ph") ? "Telegraph" : "Teletype";
                        builder.Clear();

                        builder.AppendLine($"<b>Название: </b>{ev.Name}");
                        builder.AppendLine(
                            $"Для вашего удобства мы подготовили статью в {linkType}: {ev.TelegraphLink}");

                        await client.SendTextMessageAsync(
                            chatId,
                            builder.ToString(),
                            ParseMode.Markdown);
                        keyboard = new TelegramKeyboard();
                        keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                        keyboard.AddRow("Режим нетворкинга");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Все мероприятия");
                        await client.SendTextMessageAsync(chatId, "Что нужно?", ParseMode.Markdown,
                            replyMarkup: keyboard.Markup);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            $"Вы не присоединились ни к одному мероприятию",
                            ParseMode.Markdown);
                    }

                    break;

                case "Присоединиться к мероприятию":
                    await client.SendTextMessageAsync(
                        chatId,
                        "Веедите ивент код",
                        ParseMode.Markdown);
                    await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForEventCode);
                    break;


                case "Все мероприятия":


                    var events = Utils.SplitList(2, (await eventDb.GetUserEvents(chatId)).Select(e => e.Name).ToList());


                    List<string> temp = events[0];
                    temp[0] = $"{temp[0]} {Utils.GetCheckmark()}";

                    var forKeyboard = Utils.SplitList(1, temp);

                    string cont = events.Count > 1 ? "2" : "|";

                    forKeyboard.Add(new List<string> {"|", "X", $"{cont}"});

                    keyboard = new TelegramKeyboard();

                    foreach (var list in forKeyboard)
                    {
                        keyboard.AddRow(list);
                    }


                    await client.SendTextMessageAsync(
                        chatId,
                        "Все мероприятия",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);

                    await UserDB.ChangeUserAction(context, chatId, Actions.AllEventsChangePage);

                    break;

                case "Режим нетворкинга":

                    if (user.CompanyAndPosition == null) // TODO: Проверка на то, зарегистрирован ли пользователь
                    {
                        //ОТ ЛИЗЫ
                        eventDb.AddInfoAboutUsers(chatId, "Количество активаций режима общения");
                        await UserDB.ChangeUserAction(context, chatId, Actions.FirstQuestion);
                        //КОНЕЦ ОТ ЛИЗЫ

                        // Phrase shows user that mode has changed
                        await client.SendTextMessageAsync(
                            chatId,
                            "**Где и кем вы работаете? [1/3]**\n\nДля **режима общения** жизненно необходимо ввести дополнительные сведения – **3 вопроса и 2 этапа выбора тегов** (тег – сфера деятельности человека, упрощает поиск нужных вам людей).\n\nДавайте начнём 🙃",
                            ParseMode.Markdown);
                        // First question
                        await client.SendTextMessageAsync(
                            chatId,
                            "Это поможет людям понять, чем вы можете быть им интересен. Пришли мне, пожалуйста, название компании и твою должность. __Например__, \"Дизайнер в Яндекс\"",
                            ParseMode.Markdown);
                    }
                    else
                    {
                        await UserDB.ChangeUserAction(context, chatId, Actions.Networking);
                    }

                    break;

                default:
                    await client.SendTextMessageAsync(
                        chatId,
                        "чот не то",
                        ParseMode.Markdown);
                    break;
            }
        }

        [UserAction(Actions.AllEventsChangePage)]
        public async Task OnEventsChangePage(ApplicationContext context, Message message, TelegramBotClient client)
        {
            EventDB eventDb = new EventDB();

            var text = message.Text;
            var chatId = message.Chat.Id;
            var events = Utils.SplitList(2, (await eventDb.GetUserEvents(chatId)).Select(e => e.Name).ToList());

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard;

            if (text == "X")
            {
                keyboard = new TelegramKeyboard();
                keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                keyboard.AddRow("Режим нетворкинга");
                keyboard.AddRow("Записная книжка");
                keyboard.AddRow("Все мероприятия");
                await client.SendTextMessageAsync(chatId, "Что нужно?", ParseMode.Markdown,
                    replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
            }

            if (int.TryParse(text, out int n))
            {
                int page = Convert.ToInt32(text);

                if (page <= events.Count && page > 0)
                {
                    for (var i = 0; i < events[page - 1].Count; i++)
                    {
                        string t = events[page - 1][i];
                        if (eventDb.Context.Events.Find(user.CurrentEventId).Name == t)
                        {
                            events[page - 1][i] = $"{t} {Utils.GetCheckmark()}";
                            break;
                        }
                    }


                    string left = page - 1 > 0 ? $"{page - 1}" : "|";
                    string right = page + 1 < events.Count ? $"{page + 1}" : "|";


                    var forKeyboard = Utils.SplitList(1, events[page - 1]);

                    forKeyboard.Add(new List<string> {left, "X", right});


                    keyboard = new TelegramKeyboard();

                    foreach (var list in forKeyboard)
                    {
                        keyboard.AddRow(list);
                    }

                    await client.SendTextMessageAsync(
                        chatId,
                        "Все мероприятия",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);
                }
            }
            else
            {
                if (text.Contains(Utils.GetCheckmark()))
                {
                    text = text.Substring(0, text.Length - 2);
                }


                var evs = await eventDb.GetUserEvents(chatId);
                foreach (var ev in evs)
                {
                    if (ev.Name == text)
                    {
                        if (ev.EventId == user.CurrentEventId)
                        {
                            await client.SendTextMessageAsync(chatId, "Вы уже присоединились к этому мероприятию");
                            break;
                        }

                        await client.SendTextMessageAsync(chatId, $"Вы переключили текущее мероприятие на *{ev.Name}*",
                            ParseMode.Markdown);
                        user.CurrentEventId = ev.EventId;
                        break;
                    }
                }

                context.Update(user);
                context.SaveChanges();
                keyboard = new TelegramKeyboard();
                keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                keyboard.AddRow("Режим нетворкинга");
                keyboard.AddRow("Записная книжка");
                keyboard.AddRow("Все мероприятия");
                await client.SendTextMessageAsync(chatId, "Что нужно?", ParseMode.Markdown,
                    replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
            }
        }


        #region Network mode

        [UserAction(Actions.FirstQuestion)]
        public async Task OnFirstQuestion(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            user.CompanyAndPosition = text;

            await UserDB.ChangeUserAction(context, chatId, Actions.SecondQuestion);
            await client.SendTextMessageAsync(
                chatId, "Теперь интересные вопросы😜 \n**Чем вы можете быть полезны? [2/3]**", ParseMode.Markdown);
        }

        [UserAction(Actions.SecondQuestion)]
        public async Task OnSecondQuestion(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            user.Utility = text;

            await UserDB.ChangeUserAction(context, chatId, Actions.ThirdQuestion);
            await client.SendTextMessageAsync(
                chatId, "Более отвлечённый вопрос🤗\n**О чем бы вы хотели пообщаться? [3/3]**\nТемы рабочие и не очень",
                ParseMode.Markdown);
        }

        [UserAction(Actions.ThirdQuestion)]
        public async Task OnThirdQuestion(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);

            user.Communication = text;

            // For answer

            List<Tag> parentTags = context.Tags.Where(x => x.Level == 1).ToList();

            foreach (var parentTag in parentTags)
            {
                keyboard.AddRow(parentTag.Name);
            }

            await client.SendTextMessageAsync(
                chatId, "😋 Хорошо, перейдём к **тегам**." +
                        "\n\nПо тегам можно легко и удобно __сортировать__ нужных вам людей. Сначала выбираются группы тегов, но с помощью кнопки **добавить тег** можно не ограничиваться одной группой" +
                        "\n\nВы можете их менять по кнопке мой профиль" +
                        "\n\nА сейчас выберите ВАШИ теги (подходят лично **ВАМ**) **[1/2]**",
                ParseMode.Markdown, replyMarkup: keyboard.Markup);

            await UserDB.ChangeUserAction(context, chatId, Actions.AddingParentTag);
        }

        #region Editing user

        [UserAction(Actions.AddingParentTag)]
        public async Task OnAddingTags(ApplicationContext context, Message message, TelegramBotClient client)
        {
            // Add one tag and than more tags (next Action.ChoosingTags)
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);
            Tag parentTag = context.Tags.FirstOrDefault(x => x.Name == text);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);
            TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();

            // Listing tags of this parent tag to user
            List<Tag> childTags = context.Tags.Where(x => x.Level == 2 && x.ParentTagID == parentTag.TagId).ToList();
            bool ex;
            string ch;
            foreach (var tag in childTags)
            {
                ex = user.UserTags.FirstOrDefault(e => e.TagId == tag.TagId) != null;
                if (ex)
                {
                    ch = Utils.GetCheckmark();
                }
                else
                {
                    ch = "";
                }
                
                inlineKeyboard.AddTextRow($"{tag.Name} {ch}").AddCallbackRow($"tag-{tag.TagId}");
            }

            keyboard.AddRow("Ок"); // Variants of actions
            keyboard.AddRow("Добавить тег");
            keyboard.AddRow("Выбрать заново");

            // For My profile 
//            string chosenTags = "";
//
//            if (user.UserTags.Where(x => x.UserId == user.UserId) != null)
//            {
//                foreach (var tag in user.UserTags.Where(x => x.UserId == user.UserId))
//                {
//                    chosenTags = context.Tags.FirstOrDefault(x => x.TagId == tag.TagId && x.ParentTag == null).Name + " "; 
//                }
//
//                await client.SendTextMessageAsync(
//                    chatId, "Сейчас вы выбрали: " + chosenTags, ParseMode.Markdown);
//            }

            await client.SendTextMessageAsync(
                chatId, "Выберите теги:", ParseMode.Markdown, // Editing while choosing tags?
                replyMarkup: inlineKeyboard.Markup);

            await client.SendTextMessageAsync(
                chatId, "?", ParseMode.Markdown,
                replyMarkup: keyboard.Markup);

            await UserDB.ChangeUserAction(context, chatId, Actions.ChoosingTags);
        }

        [UserAction(Actions.ChoosingTags)]
        public async Task OnChoosingTags(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);

            List<UserTag> userTags = user.UserTags;
            List<Tag> parentTags = context.Tags.Where(x => x.Level == 1).ToList();
            string chosenTags = "";
            int i = 0;

            // Gets users callbacks from inline keyboard and receiving answers from normal keyboard
            switch (text)
            {
                case "ОК": // Shows all Networking buttons 
                    if (userTags != null && userTags.Count > 0)
                    {
                        foreach (var userTag in userTags)
                        {
                            chosenTags += userTag.Tag.Name + ", ";
                        }

                        foreach (var parentTag in parentTags)
                        {
                            keyboard.AddRow(parentTag.Name);
                        }

                        await client.SendTextMessageAsync(
                            chatId, "Ваши теги:\n" + chosenTags,
                            ParseMode.Markdown);

                        await client.SendTextMessageAsync(
                            chatId,
                            "😎 Последний шаг\n\nВыберите **теги** нужных ВАМ людей (**ВЫ** их ищете) – _теги поиска_",
                            ParseMode.Markdown, replyMarkup: keyboard.Markup);

                        await UserDB.ChangeUserAction(context, chatId, Actions.SearchingParentTag);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                            chatId, "Пожалуйста, выберите теги!", ParseMode.Markdown);
                    }

                    break;

                case "Добавить тег": // Add new parent tag
                    

                    foreach (var parentTag in parentTags)
                    {
                        keyboard.AddRow(parentTag.Name);
                    }
                    

                    await client.SendTextMessageAsync(
                        chatId, "Выберите главный тег:",
                        ParseMode.Markdown, replyMarkup: keyboard.Markup);
                    
                    await UserDB.ChangeUserAction(context, chatId, Actions.AddingParentTag);

                    break;

                case "Выбрать заново": // Returns to Action with Parent Tags? Deletes all tags that user has now

                    context.UserTags.RemoveRange(userTags); // Delete ALL previous tags
                    context.SaveChanges();
                    
                    foreach (var parentTag in parentTags)
                    {
                        keyboard.AddRow(parentTag.Name);
                    }

                    await client.SendTextMessageAsync(
                        chatId, "Выберите тег:",
                        ParseMode.Markdown, replyMarkup: keyboard.Markup);
                    
                    await UserDB.ChangeUserAction(context, chatId, Actions.AddingParentTag);

                    break;
                
            }
        }

        #endregion

        // ================================= Editing Search Parametrs ================================================

        [UserAction(Actions.SearchingParentTag)]
        public async Task OnSearchingParentTag(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);
            Tag parentTag = context.Tags.FirstOrDefault(x => x.Name == text);
            

            TelegramKeyboard keyboard = new TelegramKeyboard(true);
            TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();

            // Listing tags of this parent tag to user
            List<Tag> childTags = context.Tags.Where(x => x.Level == 2 && x.ParentTagID == parentTag.TagId).ToList();

            foreach (var tag in childTags)
            {
                inlineKeyboard.AddTextRow(tag.Name).AddCallbackRow($"searchtag-{tag.TagId}");
            }

            keyboard.AddRow("Ок"); // Variants of actions
            keyboard.AddRow("Добавить тег");
            keyboard.AddRow("Выбрать заново");

            await client.SendTextMessageAsync(
                chatId, "Выберите теги:", ParseMode.Markdown, // Editing while choosing tags?
                replyMarkup: inlineKeyboard.Markup);

            await client.SendTextMessageAsync(
                chatId, "?", ParseMode.Markdown,
                replyMarkup: keyboard.Markup);

            await UserDB.ChangeUserAction(context, chatId, Actions.SearchingTags);
        }

        [UserAction(Actions.SearchingTags)]
        public async Task OnSearchingTags(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);

            List<UserSearchingTag> userTags = user.SearchingUserTags.ToList();
            List<Tag> parentTags = context.Tags.Where(x => x.Level == 1).ToList();
            string chosenTags = "";
            int i = 0;

            // Gets users callbacks from inline keyboard and receiving answers from normal keyboard
            switch (text)
            {
                case "ОК": // Shows all Networking buttons 
                    if (userTags.Count > 0)
                    {
                        foreach (var userTag in userTags)
                        {
                            chosenTags += userTag.Tag.Name + ", ";
                        }

                        keyboard.AddRow("Мой профиль");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Общение");
                        keyboard.AddRow("Вернуться на главную");

                        foreach (var parentTag in parentTags)
                        {
                            keyboard.AddRow(parentTag.Name);
                        }

                        await client.SendTextMessageAsync(
                            chatId, "Теги нужных людей:\n" + chosenTags,
                            ParseMode.Markdown);

                        await client.SendTextMessageAsync(
                            chatId, "Поздравляю 🥳 Настройка режима общения завершена!" +
                                    "\n\n📝 В **Мой профиль** вы можете редактировать всю информацию о себе и изменять теги" +
                                    "\n\n📒 В **Записной книжке** храняться выбранные контакты" +
                                    "\n\n☕️ **Общение** запустит основную функцию. В ней вы можете добавлять людей в **книжку** или приглашать их на **встречу**",
                            ParseMode.Markdown, replyMarkup: keyboard.Markup);

//                        await UserDB.ChangeUserAction(context, chatId, Actions.Networking);
                        await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                            chatId, "Пожалуйста, выберите теги!", ParseMode.Markdown);
                    }

                    break;

                case "Добавить тег": // Add new parent tag
                    foreach (var parentTag in parentTags)
                    {
                        keyboard.AddRow(parentTag.Name);
                    }
                    

                    await client.SendTextMessageAsync(
                        chatId, "Выберите главный тег:",
                        ParseMode.Markdown, replyMarkup: keyboard.Markup);
                    
                    await UserDB.ChangeUserAction(context, chatId, Actions.SearchingParentTag);

                    break;

                case "Выбрать заново": // Returns to Action with Parent Tags? Deletes all tags that user has now
                    context.UserSearchingTags.RemoveRange(userTags); // Delete ALL previous tags
                    context.SaveChanges();
                    
                    foreach (var parentTag in parentTags)
                    {
                        keyboard.AddRow(parentTag.Name);
                    }

                    await client.SendTextMessageAsync(
                        chatId, "Выберите тег:",
                        ParseMode.Markdown, replyMarkup: keyboard.Markup);
                    
                    await UserDB.ChangeUserAction(context, chatId, Actions.SearchingParentTag);

                    break;
                
            }
        }

        [UserAction(Actions.NetworkingMenu)] // Waiting for "Мой профиль", "Общение", etc.
        public async Task OnNetworkingMenu(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);

            switch (text)
            {
                case "Мой профиль":
                    break;

                case "Записная книжка":
                    // Переходит Contactbook
                    break;

                case "Общение":
                    break;

                case "Вернуться на главную":
                    break;
            }
        }

        #endregion
    }
}