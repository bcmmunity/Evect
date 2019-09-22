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
            
            try
            {
                await commands[text](context, message, client);
                return;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("\n\n\n\n\n");
                Console.WriteLine("no method for this text");
                Console.WriteLine("\n\n\n\n\n");

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
                    if (!have)
                    {
                        UserEvent userEvent = new UserEvent() {UserId = user.UserId, EventId = ev.EventId};
                        user.UserEvents.Add(userEvent);
                        user.CurrentEventId = ev.EventId;
                        //почему не работает,когда это раскоменчено?
                        context.Users.Update(user);
                        context.SaveChanges();
                    }

                    await UserDB.AdminAuthorized(context, chatId);
                    if (user.Email == null)
                    {
                        await UserDB.ChangeUserAction(context, chatId, Actions.WainingForEmail);
                        await client.SendTextMessageAsync(chatId, "Введите, пожалуйста, свою почту");
                        await UserDB.AdminAuthorized(context, chatId);
                    }
                    else
                    {
                        TelegramKeyboard keyboard = new TelegramKeyboard();
                        keyboard.AddRow("Об ивенте");
                        keyboard.AddRow("Информация о пользователях");
                        keyboard.AddRow("Создать опрос");
                        keyboard.AddRow("Создать оповещение");
                        keyboard.AddRow("Войти как обычный участник");
                         await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                        if (!have)
                        {
                            TimeToJoinToEvent time = new TimeToJoinToEvent();
                            time.TelegramId = chatId;
                            time.EventId = ev.EventId;
                            time.time = DateTime.Now;
                            context.TimeToJoinToEvents.Add(time);
                           
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
                }
                else
                {
                    if (user.IsAdminAuthorized)
                        user.IsAdminAuthorized = false;
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
                keyboard.AddRow("Количество использования режим общения");
                keyboard.AddRow("Сколько встреч согласовано");
                keyboard.AddRow("Сколько запрошено контактов");
                keyboard.AddRow("Среднее число контактов");
                keyboard.AddRow("Результаты опросов");
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
            else if (message.Text == "Создать опрос")
            {
                long chatId = message.Chat.Id;
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                keyboard.AddRow("Опрос с развернутой обратной связью");
                keyboard.AddRow("Опрос с оценкой");
                keyboard.AddRow("Результаты опросов");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Выберите тип опроса", replyMarkup: keyboard.Markup);
            }
            else if(message.Text=="Войти как обычный участник")
            {
                long chatId = message.Chat.Id;
               await UserDB.ChangeUserAction(context, chatId, Actions.None);
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Войти по ивент-коду");
                keyboard.AddRow("Личный кабинет");
                User user = context.Users.FirstOrDefault(N => N.TelegramId == chatId);
                user.IsAdminAuthorized = false;
                await client.SendTextMessageAsync(chatId, "У вас есть личный кабинет? Если нет, то войдите по *ивент-коду* \n P.S.*Ивент-код* отправлен в письме регистрации", ParseMode.Markdown, replyMarkup: keyboard.Markup);
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
                keyboard.AddRow("Информация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");
                keyboard.AddRow("Войти как обычный участник");
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
                keyboard.AddRow("Информация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");
                keyboard.AddRow("Войти как обычный участник");
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
                    keyboard.AddRow("Информация о пользователях");
                    keyboard.AddRow("Создать опрос");
                    keyboard.AddRow("Создать оповещение");
                    keyboard.AddRow("Войти как обычный участник");
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
                case "Количество использования режим общения":
                {
                    string amountOfActivationOfNetworkingMode =
                        await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                    await client.SendTextMessageAsync(chatId, amountOfActivationOfNetworkingMode);
                }
                    break;
                case "Сколько встреч согласовано":
                    {
                        string amountOfCompletedMeetings = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfCompletedMeetings);
                    }
                    break;
                case "Сколько запрошено контактов":
                    {
                        string amountOfRequestsOfContacts = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, amountOfRequestsOfContacts);
                    }
                    break;
                case "Среднее число контактов":
                    {
                        string averageOfContacts = await eventDb.GetInfrormationAboutUsers(chatId, message.Text);
                        await client.SendTextMessageAsync(chatId, averageOfContacts);
                    }
                    break;
             /*   case "Число запросов контактов":
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
/*
                case "Результаты опросов":
                {

                }
                    break;*/
            }
        }

        [UserAction(Actions.CreateSurvey)]
        public async Task CreatingSurvey(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            if (text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Об ивенте");
                keyboard.AddRow("Информация о пользователях");
                keyboard.AddRow("Создать опрос");
                keyboard.AddRow("Создать оповещение");
                await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                await client.SendTextMessageAsync(chatId, "Вы вернулись в главное меню", replyMarkup: keyboard.Markup);
            }
            else if (text == "Опрос с развернутой обратной связью")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                
                await UserDB.ChangeUserAction(context, chatId, Actions.SurveyWithMessage);
                await client.SendTextMessageAsync(chatId, "Отправьте вопрос,он будет отправлен всем", replyMarkup: keyboard.Markup);
            }
            else if (text == "Опрос с оценкой")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
               
                
                await client.SendTextMessageAsync(chatId, "Напишите вопрос,он будет отправлен всем", replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.SurveyWithMarks);
            }
            else if(text=="Результаты опросов")
            {

            }

        }

        [UserAction(Actions.SurveyWithMessage)]
        public async Task OnCreatingWithMessage(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            if (message.Text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                keyboard.AddRow("Опрос с развернутой обратной связью");
                keyboard.AddRow("Опрос с оценкой");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Вы вернулись к выбору типа опроса",
                    replyMarkup: keyboard.Markup);
            }
            else
            {
                TelegramKeyboard keyboard = Utils.CommonKeyboards(Actions.AdminMode);
               await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                UserDB.AddSurvey(context, 0, message.Text, chatId);
                EventDB eventDb = new EventDB();
                List<long> allParticipants = await eventDb.GetAllParticipantsOfEvent(chatId);
                int QuestionId = UserDB.GetQuestionId(context, message.Text);
                TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();
                inlineKeyboard.AddTextRow("Ответить").AddCallbackRow("999-" + QuestionId.ToString());
                foreach (var participant in allParticipants)
                {
                    await client.SendTextMessageAsync(participant, message.Text, replyMarkup: inlineKeyboard.Markup);
                }
                await client.SendTextMessageAsync(chatId, "Спасибо, Ваш вопрос успешно разослан участникам", replyMarkup: keyboard.Markup);
            }
        }

        [UserAction(Actions.SurveyWithMarks)]
        public async Task OnCreatingSurveyWithMarks(ApplicationContext context, Message message,
            TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            if (message.Text == "Назад")
            {
                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Назад");
                keyboard.AddRow("Опрос с развернутой обратной связью");
                keyboard.AddRow("Опрос с оценкой");
                await UserDB.ChangeUserAction(context, chatId, Actions.CreateSurvey);
                await client.SendTextMessageAsync(chatId, "Я вернулся к выбору типа опроса",
                    replyMarkup: keyboard.Markup);
            }
            else
            {
                TelegramKeyboard keyboard = Utils.CommonKeyboards(Actions.AdminMode);
                 UserDB.AddSurvey(context, 1, message.Text, chatId);
                EventDB eventDb = new EventDB();
                List<long> allParticipants =await eventDb.GetAllParticipantsOfEvent(chatId);
                int QuestionId = UserDB.GetQuestionId(context, message.Text);
                TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();
                inlineKeyboard
                    .AddTextRow("🔥".ToString(), "👍".ToString(), "👌".ToString(), "👎".ToString(), "🤢".ToString())
                    .AddCallbackRow("990-" + "1-" + QuestionId.ToString(), "990-" + "2-" + QuestionId.ToString(),
                        "990-" + "3-" + QuestionId.ToString(), "990-" + "4-" + QuestionId.ToString(),
                        "990-" + "5-" + QuestionId.ToString());
                foreach (var participant in allParticipants)
                {
                    await client.SendTextMessageAsync(participant, message.Text, replyMarkup: inlineKeyboard.Markup);
                }
                await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                await client.SendTextMessageAsync(chatId, "Ваш вопрос успешно всем разослан", replyMarkup: keyboard.Markup);
            }
        }

             
        [UserAction(Actions.AnswerToSurvey)]
        public async Task onAnsweringToSurvey(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            User user = context.Users.FirstOrDefault(n => n.TelegramId == chatId);
            if (message.Text == "Продолжить предыдущие действия")
            {
                Actions action = user.PreviousAction;
                TelegramKeyboard keyboard = new TelegramKeyboard();
                if (Utils.CommonKeyboards(action) != null)
                {
                    keyboard = Utils.CommonKeyboards(action);
                }

                user.PreviousAction = new Actions();
                await UserDB.ChangeUserAction(context, chatId, action);
                if (keyboard != null)
                {
                    await client.SendTextMessageAsync(chatId, "Теперь вы можете продолжать работу",
                        replyMarkup: keyboard.Markup);
                }
                else await client.SendTextMessageAsync(chatId, "Теперь вы можете продолжать работу");
            }
            else if (message.Text == "Написать еще один ответ")
            {
                await client.SendTextMessageAsync(chatId, "Отправьте сообщение,оно будет записано как ответ на вопрос");
            }
            else
            {
                Answer answer = new Answer();
                answer.AnswerMessage = message.Text;
                answer.QuestionId = user.CurrentQuestionId;
                answer.TelegramId = chatId;
                context.Answers.Add(answer);
                context.SaveChanges();
                TelegramKeyboard keyboard = new TelegramKeyboard(true);
                keyboard.AddRow("Продолжить предыдущие действия");
                keyboard.AddRow("Написать еще один ответ");
                await client.SendTextMessageAsync(chatId, "Спасибо, Ваш ответ сохранён\nВыберите, что делать дальше",
                    replyMarkup: keyboard.Markup);
            }
        }

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
                if (user.FirstName == null && user.LastName == null)
                {
                    user.FirstName = names[0].Correct();
                    user.LastName = names[1].Correct();
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
                    user.FirstName = names[0].Correct();
                    user.LastName = names[1].Correct();
                    context.Update(user);
                    context.SaveChanges();

                    await client.SendTextMessageAsync(chatId, "Данные сохранены");

                    TelegramKeyboard keyboard = new TelegramKeyboard();

                    keyboard.AddRow("Имя и фамилия");
                    keyboard.AddRow("Почта");
                    keyboard.AddRow("Работа и должность");
                    keyboard.AddRow("Полезность");
                    keyboard.AddRow("О чем пообщаться");
                    keyboard.AddRow("Вернуться в мой профиль");

                    await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);
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
            TelegramKeyboard keyboard;
            if (Utils.IsEmailValid(text))
            {
                if (await UserDB.CheckEmailInDB(context, text) && text != user.Email)
                {
                    await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForValidationCode);

                    await client.SendTextMessageAsync(
                        chatId,
                        "Пользователь с этой почтой ранее использовал другой аккаунт телеграм. На эту почту отпарвлен код идентификации. Пожалуйста введите код",
                        ParseMode.Markdown);

                    string code = Utils.GenerateRandomCode();
                    await Utils.SendEmailAsync(text, "Потверждение почты", $"Ваш кода для потверждения почты: {code}");
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
                    if (user.Email == null)
                    {
                        user.Email = text.Correct();
                        context.Update(user);
                        context.SaveChanges();
                        if (user.IsAdminAuthorized)
                        {
                            await UserDB.ChangeUserAction(context, chatId, Actions.AdminMode);
                            keyboard = new TelegramKeyboard();
                            keyboard.AddRow("Об ивенте");
                            keyboard.AddRow("Информация о пользователях");
                            keyboard.AddRow("Создать опрос");
                            keyboard.AddRow("Создать оповещение");
                            keyboard.AddRow("Войти как обычный участник");
                            StringBuilder builder = new StringBuilder();
                            int evId = user.CurrentEventId;
                            Event eventt = context.Events.FirstOrDefault(n => n.EventId == evId);
                            builder.AppendLine($"Включён режим организатора на мероприятии \"{eventt.Name}\"");
                            await client.SendTextMessageAsync(chatId, builder.ToString(), replyMarkup: keyboard.Markup);
                        }
                        else
                        {
                            keyboard = new TelegramKeyboard();
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
                        user.Email = text.Correct();
                        context.Update(user);
                        context.SaveChanges();


                        keyboard = new TelegramKeyboard();

                        await client.SendTextMessageAsync(chatId, "Данные сохранены");
                        keyboard.AddRow("Имя и фамилия");
                        keyboard.AddRow("Почта");
                        keyboard.AddRow("Работа и должность");
                        keyboard.AddRow("Полезность");
                        keyboard.AddRow("О чем пообщаться");
                        keyboard.AddRow("Вернуться в мой профиль");

                        await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                            replyMarkup: keyboard.Markup);
                        await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);
                    }
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
                    "Вы успешно прекратили пользоваться evectbot, для того чтобы начать заново напишите _/start_",
                    ParseMode.Markdown);
            }
            else if (text == "Нет")
            {
                context.Users.Remove(user);
                context.SaveChanges();
                await client.SendTextMessageAsync(
                    chatId,
                    "Вся информация удалена, для того чтобы начать заново напишите _/start_",
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

                        builder.AppendLine($"*Название: *{ev.Name}");
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
                            "*Где и кем вы работаете? [1/3]*\n\nДля *режима общения* жизненно необходимо ввести дополнительные сведения – *3 вопроса и 2 этапа выбора тегов* (тег – сфера деятельности человека, упрощает поиск нужных вам людей).\n\nДавайте начнём 🙃",
                            ParseMode.Markdown);
                        // First question
                        await client.SendTextMessageAsync(
                            chatId,
                            "Это поможет людям понять, чем вы можете быть им интересен. Пришли мне, пожалуйста, название компании и твою должность. __Например__, \"Дизайнер в Яндекс\"",
                            ParseMode.Markdown);
                    }
                    else
                    {
                        keyboard = new TelegramKeyboard();

                        keyboard.AddRow("Мой профиль");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Общение");
                        keyboard.AddRow("Вернуться на главную");

                        await UserDB.ChangeUserAction(context, chatId, Actions.NetworkingMenu);
                        await client.SendTextMessageAsync(chatId, "Вы вошли в режим нетворкинга",
                            replyMarkup: keyboard.Markup);
                    }

                    break;

                case "Записная книжка":
                    List<ContactsBook> contacts = user.Contacts.Take(4).ToList();
                    List<string> nums = new List<string>(4);
                    List<string> ids = new List<string>(4);


                    var tags = user.SearchingUserTags.Select(u =>
                        context.SearchingTags.FirstOrDefault(t => t.SearchingTagId == u.TagId)?.Name).ToList();

                    builder.AppendLine("_Ваши теги_");
                    builder.AppendLine($"`{string.Join(", ", tags)}`");
                    builder.AppendLine();
                    builder.AppendLine("*Ваши контакты*");
                    builder.AppendLine();
                    int i = 1;

                    foreach (var contactsBook in contacts)
                    {
                        nums.Add(i.ToString());
                        ids.Add($"prof-{contactsBook.AnotherUserId}");
                        User another = await UserDB.GetUserByChatId(context, contactsBook.AnotherUserId);
                        builder.AppendLine(
                            $"*{i})*{another.FirstName} {another.LastName} {another.CompanyAndPosition}");
                        builder.AppendLine($"_Чем полезен_: {another.Utility}");
                        builder.AppendLine($"_Контакт_: @{another.TelegramUserName}");
                        builder.AppendLine();
                        i++;
                    }

                    TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
                    if (user.Contacts.Count > 4)
                    {
                        inline
                            .AddTextRow("Вперед")
                            .AddCallbackRow("profpage-2");
                    }

                    inline
                        .AddTextRow(nums.ToArray())
                        .AddCallbackRow(ids.ToArray());

                    await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown,
                        replyMarkup: inline.Markup);
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

            if (user.CompanyAndPosition == null)
            {
                user.CompanyAndPosition = text.Correct();

                await UserDB.ChangeUserAction(context, chatId, Actions.SecondQuestion);
                await client.SendTextMessageAsync(
                    chatId, "Теперь интересные вопросы😜 \n*Чем вы можете быть полезны? [2/3]*", ParseMode.Markdown);
            }
            else
            {
                user.CompanyAndPosition = text.Correct();

                context.Update(user);
                context.SaveChanges();

                await client.SendTextMessageAsync(chatId, "Данные сохранены");

                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Имя и фамилия");
                keyboard.AddRow("Почта");
                keyboard.AddRow("Работа и должность");
                keyboard.AddRow("Полезность");
                keyboard.AddRow("О чем пообщаться");
                keyboard.AddRow("Вернуться в мой профиль");

                await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                    replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);
            }
        }

        [UserAction(Actions.SecondQuestion)]
        public async Task OnSecondQuestion(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;


            User user = await UserDB.GetUserByChatId(context, chatId);

            if (user.Utility == null)
            {
                user.Utility = text.Correct();
                await UserDB.ChangeUserAction(context, chatId, Actions.ThirdQuestion);
                await client.SendTextMessageAsync(
                    chatId,
                    "Более отвлечённый вопрос🤗\n*О чем бы вы хотели пообщаться? [3/3]*\nТемы рабочие и не очень",
                    ParseMode.Markdown);
            }
            else
            {
                user.Utility = text.Correct();
                context.Update(user);
                context.SaveChanges();

                TelegramKeyboard keyboard = new TelegramKeyboard();
                keyboard.AddRow("Имя и фамилия");
                keyboard.AddRow("Почта");
                keyboard.AddRow("Работа и должность");
                keyboard.AddRow("Полезность");
                keyboard.AddRow("О чем пообщаться");
                keyboard.AddRow("Вернуться в мой профиль");

                await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                    replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);
            }
        }

        [UserAction(Actions.ThirdQuestion)]
        public async Task OnThirdQuestion(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await UserDB.GetUserByChatId(context, chatId);

            TelegramKeyboard keyboard = new TelegramKeyboard(true);

            if (user.Communication == null)
            {
                user.Communication = text.Correct();
                List<Tag> parentTags = context.Tags.Where(x => x.Level == 1).ToList();

                foreach (var parentTag in parentTags)
                {
                    keyboard.AddRow(parentTag.Name);
                }

                await client.SendTextMessageAsync(
                    chatId, "😋 Хорошо, перейдём к *тегам*." +
                            "\n\nПо тегам можно легко и удобно __сортировать__ нужных вам людей. Сначала выбираются группы тегов, но с помощью кнопки *добавить тег* можно не ограничиваться одной группой" +
                            "\n\nВы можете их менять по кнопке мой профиль" +
                            "\n\nА сейчас выберите ВАШИ теги (подходят лично *ВАМ*) *[1/2]*",
                    ParseMode.Markdown, replyMarkup: keyboard.Markup);

                await UserDB.ChangeUserAction(context, chatId, Actions.AddingParentTag);
            }
            else
            {
                user.Communication = text.Correct();

                context.Update(user);
                context.SaveChanges();

                keyboard.AddRow("Имя и фамилия");
                keyboard.AddRow("Почта");
                keyboard.AddRow("Работа и должность");
                keyboard.AddRow("Полезность");
                keyboard.AddRow("О чем пообщаться");
                keyboard.AddRow("Вернуться в мой профиль");

                await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                    replyMarkup: keyboard.Markup);
                await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);
            }
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
                case "Ок": // Shows all Networking buttons 
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

                        if (user.SearchingUserTags.Count == 0)
                        {
                            await client.SendTextMessageAsync(
                                chatId,
                                "😎 Последний шаг\n\nВыберите *теги* нужных ВАМ людей (*ВЫ* их ищете) – _теги поиска_",
                                ParseMode.Markdown, replyMarkup: keyboard.Markup);

                            await UserDB.ChangeUserAction(context, chatId, Actions.SearchingParentTag);
                        }
                        else
                        {
                            keyboard = new TelegramKeyboard();
                            keyboard.AddRow("Редактировать профиль");
                            keyboard.AddRow("Изменить теги");
                            keyboard.AddRow("Вернуться в режим общения");
                            await client.SendTextMessageAsync(chatId, "Вы вернулись в мой профиль",
                                replyMarkup: keyboard.Markup);
                        }
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
            List<SearchingTag> childTags = context.SearchingTags
                .Where(x => x.Level == 2 && x.ParentTagID == parentTag.TagId).ToList();

            bool ex;
            string ch;
            foreach (var tag in childTags)
            {
                ex = user.SearchingUserTags.FirstOrDefault(e => e.TagId == tag.SearchingTagId) != null;
                if (ex)
                {
                    ch = Utils.GetCheckmark();
                }
                else
                {
                    ch = "";
                }

                inlineKeyboard.AddTextRow($"{tag.Name} {ch}").AddCallbackRow($"searchtag-{tag.SearchingTagId}");
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
                case "Ок": // Shows all Networking buttons 
                    if (userTags.Count > 0)
                    {
                        keyboard.AddRow("Мой профиль");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Общение");
                        keyboard.AddRow("Вернуться на главную");

                        await client.SendTextMessageAsync(
                            chatId, "Теги нужных людей:\n" + string.Join(", ", userTags),
                            ParseMode.Markdown);

                        await client.SendTextMessageAsync(
                            chatId, "Поздравляю 🥳 Настройка режима общения завершена!" +
                                    "\n\n📝 В *Мой профиль* вы можете редактировать всю информацию о себе и изменять теги" +
                                    "\n\n📒 В *Записной книжке* храняться выбранные контакты" +
                                    "\n\n☕️ *Общение* запустит основную функцию. В ней вы можете добавлять людей в *книжку* или приглашать их на *встречу*",
                            ParseMode.Markdown, replyMarkup: keyboard.Markup);

                        await UserDB.ChangeUserAction(context, chatId, Actions.NetworkingMenu);
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
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
            StringBuilder builder = new StringBuilder();

            switch (text)
            {
                case "Мой профиль":

                    keyboard.AddRow("Редактировать профиль");
                    keyboard.AddRow("Изменить теги");
                    keyboard.AddRow("Вернуться в режим общения");

                    List<string> myTags = user.UserTags
                        .Select(u => context.Tags.FirstOrDefault(t => t.TagId == u.TagId)?.Name).ToList();
                    List<string> searchTags = user.SearchingUserTags.Select(u =>
                        context.SearchingTags.FirstOrDefault(t => t.SearchingTagId == u.TagId)?.Name).ToList();


                    builder.AppendLine($"_Имя и фамилия_: {user.FirstName} {user.LastName}");
                    builder.AppendLine($"_Компания и позиция_: {user.CompanyAndPosition}");
                    builder.AppendLine();
                    builder.AppendLine($"_Чем полезен_: {user.Utility}");
                    builder.AppendLine($"_О чем можете пообщаться_: {user.Communication}");
                    builder.AppendLine();
                    builder.AppendLine("Личные теги: ");
                    builder.AppendLine($"`{string.Join(", ", myTags)}`");
                    builder.AppendLine();
                       builder.AppendLine("Теги для поиска: ");
                    builder.AppendLine($"`{string.Join(", ", searchTags)}`");

                    await client.SendTextMessageAsync(chatId, builder.ToString(), replyMarkup: keyboard.Markup,
                        parseMode: ParseMode.Markdown);
                    await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileMenu);

                    break;

                case "Записная книжка":
                    // Переходит Contactbook

                    List<ContactsBook> contacts = user.Contacts.Take(4).ToList();
                    List<string> nums = new List<string>(4);
                    List<string> ids = new List<string>(4);


                    var tags = user.SearchingUserTags.Select(u =>
                        context.SearchingTags.FirstOrDefault(t => t.SearchingTagId == u.TagId)?.Name).ToList();

                    builder.AppendLine("_Ваши теги_");
                    builder.AppendLine($"`{string.Join(", ", tags)}`");
                    builder.AppendLine();
                    builder.AppendLine("*Ваши контакты*");
                    builder.AppendLine();
                    int i = 1;

                    foreach (var contactsBook in contacts)
                    {
                        nums.Add(i.ToString());
                        ids.Add($"prof-{contactsBook.AnotherUserId}");
                        User another = await UserDB.GetUserByChatId(context, contactsBook.AnotherUserId);
                        builder.AppendLine(
                            $"*{i})*{another.FirstName} {another.LastName} {another.CompanyAndPosition}");
                        builder.AppendLine($"_Чем полезен_: {another.Utility}");
                        builder.AppendLine($"_Контакт_: " + (another.TelegramUserName != null ? "@" + another.TelegramUserName : "[inline mention of a user](tg://user?id={chatId})"));
                        builder.AppendLine();
                        i++;
                    }

                    inline = new TelegramInlineKeyboard();
                    if (user.Contacts.Count > 4)
                    {
                        inline
                            .AddTextRow("Вперед")
                            .AddCallbackRow("profpage-2");
                    }

                    inline
                        .AddTextRow(nums.ToArray())
                        .AddCallbackRow(ids.ToArray());

                    await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown,
                        replyMarkup: inline.Markup);

                    break;

                case "Общение":

                    // поиск по тегам, выбираем из всех пользователей только тех, у которых есть хотя бы один личный тег из тех, что у нас в поиске
                    User us = context.Users
                    .Include(u => u.UserTags)
                    .ThenInclude(t => t.Tag)
                    .FirstOrDefault(e =>
                        e.UserTags.Any(ut =>
                            user.SearchingUserTags.FirstOrDefault(t => t.TagId == ut.TagId) != null) &&
                        e.CurrentEventId == user.CurrentEventId);

                    
                    builder.AppendLine($@"{us.FirstName},  {us.CompanyAndPosition}");
                    builder.AppendLine();
                    builder.AppendLine($"_Теги_: `{string.Join(", ",us.UserTags.Select(e => e.Tag.Name))}`");
                    builder.AppendLine();
                    builder.AppendLine($"_Чем полезен_: {us.Utility}");
                    builder.AppendLine();
                    builder.AppendLine($"_О чем можно пообщаться_: {us.Communication}");

                    string ch;

                    if (user.Contacts.Any(e => e.AnotherUserId == us.TelegramId))
                    {
                        ch = Utils.GetCheckmark();
                    }
                    else
                    {
                        ch = "В книжку";
                    }

                    inline = new TelegramInlineKeyboard();
                    inline
                        .AddTextRow("Назад", ch, "Встреча", "Вперед")
                        .AddCallbackRow($"change-0", $"contact-{us.TelegramId}", $"meet-{us.TelegramId}", $"change-2");

                    await client.SendTextMessageAsync(chatId, builder.ToString(), ParseMode.Markdown,
                        replyMarkup: inline.Markup);

                    break;

                case "Вернуться на главную":
                    keyboard = new TelegramKeyboard();
                    keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                    keyboard.AddRow("Режим нетворкинга");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Все мероприятия");

                    await client.SendTextMessageAsync(
                        chatId,
                        "Вы вернулись в главное меню",
                        ParseMode.Markdown,
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.Profile);
                    break;
            }
        }

        [UserAction(Actions.MyProfileMenu)]
        public async Task OnMyProfileMenu(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            TelegramKeyboard keyboard = new TelegramKeyboard();

            switch (text)
            {
                case "Редактировать профиль":
                    keyboard.AddRow("Имя и фамилия");
                    keyboard.AddRow("Почта");
                    keyboard.AddRow("Работа и должность");
                    keyboard.AddRow("Полезность");
                    keyboard.AddRow("О чем пообщаться");
                    keyboard.AddRow("Вернуться в мой профиль");
                    await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.MyProfileEditing);

                    break;

                case "Изменить теги":
                    keyboard.AddRow("Теги поиска");
                    keyboard.AddRow("Личные теги");
                    keyboard.AddRow("Вернуться в мой профиль");
                    await client.SendTextMessageAsync(chatId, "Выберите пункты для редактирования",
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.TagsEditing);
                    break;

                case "Вернуться в режим общения":
                    keyboard.AddRow("Мой профиль");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Общение");
                    keyboard.AddRow("Вернуться на главную");

                    await client.SendTextMessageAsync(chatId, "Вы вернулись в режим общения",
                        replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.NetworkingMenu);

                    break;
            }
        }

        [UserAction(Actions.TagsEditing)]
        public async Task OnTagsEditing(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            TelegramKeyboard keyboard = new TelegramKeyboard();

            List<Tag> parentTags = context.Tags.Where(x => x.Level == 1).ToList();

            foreach (var parentTag in parentTags)
            {
                keyboard.AddRow(parentTag.Name);
            }

            switch (text)
            {
                case "Теги поиска":
                    await client.SendTextMessageAsync(chatId, "Выберите тему", replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.SearchingParentTag);
                    break;

                case "Личные теги":
                    await client.SendTextMessageAsync(chatId, "Выберите тему", replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.AddingParentTag);
                    break;

                case "Вернуться в мой профиль":
                    keyboard.AddRow("Мой профиль");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Общение");
                    keyboard.AddRow("Вернуться на главную");

                    await client.SendTextMessageAsync(chatId, "Вы вернулись в профиль", replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.NetworkingMenu);
                    break;
            }
        }

        [UserAction(Actions.MyProfileEditing)]
        public async Task OnMyProfileEditing(ApplicationContext context, Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            TelegramKeyboard keyboard = new TelegramKeyboard();

            switch (text)
            {
                case "Имя и фамилия":
                    await client.SendTextMessageAsync(chatId, "Введите новые данные:");
                    await UserDB.ChangeUserAction(context, chatId, Actions.WaitingForName);
                    break;

                case "Почта":
                    await client.SendTextMessageAsync(chatId, "Введите новые данные:");
                    await UserDB.ChangeUserAction(context, chatId, Actions.WainingForEmail);
                    break;

                case "Работа и должность":
                    await client.SendTextMessageAsync(chatId, "Введите новые данные:");
                    await UserDB.ChangeUserAction(context, chatId, Actions.FirstQuestion);
                    break;

                case "Полезность":
                    await client.SendTextMessageAsync(chatId, "Введите новые данные:");
                    await UserDB.ChangeUserAction(context, chatId, Actions.SecondQuestion);
                    break;

                case "О чем пообщаться":
                    await client.SendTextMessageAsync(chatId, "Введите новые данные:");
                    await UserDB.ChangeUserAction(context, chatId, Actions.ThirdQuestion);
                    break;

                case "Вернуться в мой профиль":
                    keyboard.AddRow("Мой профиль");
                    keyboard.AddRow("Записная книжка");
                    keyboard.AddRow("Общение");
                    keyboard.AddRow("Вернуться на главную");

                    await client.SendTextMessageAsync(chatId, "Вы вернулись в профиль", replyMarkup: keyboard.Markup);
                    await UserDB.ChangeUserAction(context, chatId, Actions.NetworkingMenu);
                    break;
            }
        }

        #endregion
    }
}