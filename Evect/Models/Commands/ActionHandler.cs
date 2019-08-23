using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public async void OnNone(Message message, TelegramBotClient client)
        {
            var commands = Bot.Commands;
            var chatId = message.Chat.Id;
            var text = message.Text;

            foreach (var pair in commands)
            {
                if (pair.Value == text)
                {
                    pair.Key(message, client);
                    return;
                } 
            }



            await client.SendTextMessageAsync(
                chatId,
                "Я не понимаю вас",
                ParseMode.Html);
        }

        
        [UserAction(Actions.WaitingForEventCode)]
        public async void OnWaitingEventCode(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            EventDB eventDb = new EventDB();
            UserDB userDb = new UserDB();

            User user = await userDb.GetUserByChatId(chatId);
            bool isValid = await eventDb.IsEventCodeValid(text);
            if (isValid)
            {
                Event ev = await userDb.Context.Events.FirstOrDefaultAsync(e =>
                    e.EventCode == text || e.AdminCode == text);
                
                bool have = user.UserEvents.FirstOrDefault(ue => ue.EventId == ev.EventId) != null;
                
                bool isAdminCode = await eventDb.IsAdminCode(text);
                
                
                if(isAdminCode)
                {
                    string[][] adminActions = { new[] { "Об ивенте" }, new[] { "Информация о пользователях" }, new[] { "Создать опрос" }, new[] { "Создать оповещение" } };
                    userDb.AdminAuthorized(chatId);
                    userDb.ChangeUserAction(chatId, Actions.AdminMode);
                    if(!have)
                    {
                        UserEvent userEvent = new UserEvent() { UserId = user.UserId, EventId = ev.EventId };
                        user.UserEvents.Add(userEvent);
                        user.CurrentEventId = ev.EventId;
                        //почему не работает,когда это раскоменчено?
                        //userDb.Context.Users.Update(user);
                        //await userDb.Context.SaveChangesAsync();
                    }
                    await client.SendTextMessageAsync(chatId, $"Включён режим организатора на мероприятии \"{ev.Name}\"" + "😇".ToString() + "\n" + "Вам доступен расширенный функционал:\n\n" + "0️⃣".ToString() + "<b>Об ивенте</b>- внести изменение в информацию о мероприятии" + "1️⃣".ToString() + "Можно получить <b>информацию по всем участникам</b>" + "2️⃣".ToString() + "<b>Создать опрос</b>- опрос рассылается всем участникам, тип опроса- оценка от 1 до 5"+ "3️⃣".ToString()+"<b>Создать оповещение</b>- сообщение отправляется всем участникам",ParseMode.Html, replyMarkup:TelegramKeyboard.GetKeyboard(adminActions));                  
                }
                else
                {
                    if (have)
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            "Вы уже присоединились к этому мероприятию",
                            ParseMode.Html); 
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
                        userDb.Context.Users.Update(user);
                        await userDb.Context.SaveChangesAsync();

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
                }
                

            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    $"Неправильный код(",
                    ParseMode.Html);
            }
        }
        [UserAction(Actions.AdminMode)]
        public async void AdminMode(Message message,TelegramBotClient client)
        {
            if(message.Text=="Об ивенте")
            {
                EventDB eventDB = new EventDB();
                long chatId = message.Chat.Id;
                UserDB userDb = new UserDB();
                User user = await userDb.GetUserByChatId(chatId);
                if (!user.IsAdminAuthorized)
                {
                    await client.SendTextMessageAsync(chatId, "Я не знаю такой команды пока");
                }
                else
                {
                    string[][] back = { new[] { "Назад" } };
                    userDb.ChangeUserAction(chatId, Actions.GetInformationAboutTheEvent);
                    string info = await eventDB.GetInformationAboutEvent(chatId);
                    await client.SendTextMessageAsync(chatId, info, replyMarkup: TelegramKeyboard.GetKeyboard(back));
                }
            }
            if(message.Text=="Информация о пользователях")
            {

            }
            if(message.Text=="Создать оповещение")
            {

            }
            if(message.Text=="Создать опрос")
            {

            }
        }
        [UserAction(Actions.GetInformationAboutTheEvent)]
        public async void InformationAboutTheEvent(Message message,TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            UserDB userDb = new UserDB();
            if(text=="Назад")
            {
               string [][] menu = { new[] { "Об ивенте" }, new[] { "Информация о пользователях" }, new[] { "Создать опрос" }, new[] { "Создать оповещение" } };
                userDb.ChangeUserAction(chatId, Actions.AdminMode);
                await client.SendTextMessageAsync(chatId, "Я вернулся в главное меню", replyMarkup: TelegramKeyboard.GetKeyboard(menu));
            }
            else
            {
                EventDB eventDb = new EventDB();
                eventDb.AddInformationAboutEvent(chatId,text);
                await client.SendTextMessageAsync(chatId, "Данные о мероприятии успешно сохранены");
            }
            
        }
       
        [UserAction(Actions.WaitingForName)]
        public async void OnWaitingForName(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            EventDB eventDb = new EventDB();
            UserDB userDb = new UserDB();

            User user = await userDb.GetUserByChatId(chatId);

            var names = text.Split(' ');
            if (names.Length == 2)
            {
                user.FirstName = names[0];
                user.LastName = names[1];
                userDb.Context.Update(user);
                await userDb.Context.SaveChangesAsync();
                if (string.IsNullOrEmpty(user.Email))
                {
                    await client.SendTextMessageAsync(
                        chatId,
                        "Вот мы и познакомились, а теперь можно ваш адрес электронной почты?",
                        ParseMode.Html);
                    userDb.ChangeUserAction(chatId, Actions.WainingForEmail);
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Введите пожалуйста корректно имя и фамилию",
                    ParseMode.Html);
            }
        }

        [UserAction(Actions.WainingForEmail)]
        public async void OnWaitingForEmail(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            UserDB userDb = new UserDB();

            User user = await userDb.GetUserByChatId(chatId);
            
            if (Utils.IsEmailValid(text))
            {
                user.Email = text;
                userDb.Context.Update(user);
                await userDb.Context.SaveChangesAsync();
                
                string[][] actions = { new[] { "О мероприятии", "Присоединиться к мероприятию" }, new[] {"Режим нетворкинга"}, new[] {"Записная книжка"}, new[] {"Все мероприятия"} };

                await client.SendTextMessageAsync(
                    chatId,
                    "Прекрасно, вам доступен весь мой функционал",
                    ParseMode.Html,
                    replyMarkup: TelegramKeyboard.GetKeyboard(actions));
                userDb.ChangeUserAction(chatId, Actions.Profile);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Введите, пожалуйста, почту корректно",
                    ParseMode.Html);
            }
            
        }
        
        [UserAction(Actions.DeleteOrNot)]
        public async void OnDeleteOrNot(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;
            UserDB userDb = new UserDB();

            User user = await userDb.GetUserByChatId(chatId);
            
            if (text == "Да")
            {
                userDb.UserLogoff(chatId);
                userDb.ResetAction(chatId);
                await client.SendTextMessageAsync(
                    chatId,
                    "Вы успешно прекратили пользоваться evectbot, для того чтобы начать заново напишите <em>/start</em>",
                    ParseMode.Html);
            }
            else if (text == "Нет")
            {
                userDb.Context.Users.Remove(user);
                await userDb.Context.SaveChangesAsync();
                await client.SendTextMessageAsync(
                    chatId,
                    "Вся информация удалена, для того чтобы начать заново напишите <em>/start</em>",
                    ParseMode.Html);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId,
                    "Да/Нет",
                    ParseMode.Html);
            }
        }
        
        
        [UserAction(Actions.Profile)]
        public async void OnProfile(Message message, TelegramBotClient client)
        {
            UserDB userDb = new UserDB();
            EventDB eventDb = new EventDB();
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
                        string[][] actions = { new[] { "О мероприятии", "Присоединиться к мероприятию" }, new[] {"Режим нетворкинга"}, new[] {"Записная книжка"}, new[] {"Все мероприятия"} };
                        await client.SendTextMessageAsync(chatId, "Что нужно?",ParseMode.Html,replyMarkup:TelegramKeyboard.GetKeyboard(actions, true));
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
                
                
                case "Все мероприятия":
                    StringBuilder builder = new StringBuilder();

                    for (int i = 0; i < user.UserEvents.Count; i++)
                    {
                        Event ev = await eventDb.GetEventByUserEvent(user.UserEvents[i]);
                        string cur = user.CurrentEventId == ev.EventId ? "<em>(Текущее)</em>" : "";
                        builder.AppendLine($"<b>{i+1}:</b> {ev.Name} {cur}");
                    }

                    var data = new string[][] {new string[] {"1", "2", "3", "4"}};
                    var data1 = new string[][] {new string[] {"anime", "colbek1", "colbek2", "meow"}};
                    
                    await client.SendTextMessageAsync(
                        chatId,
                        builder.ToString(),
                        ParseMode.Html,
                        replyMarkup: TelegramKeyboard.GetInlineKeyboard(data, data1));
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