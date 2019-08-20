using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evect.Models;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using User = Evect.Models.User;

namespace Evect.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _db;
        private UserDB _userDb;
        private EventDB _eventDb;

        public HomeController(ApplicationContext db)
        {
            _db = db;
            _userDb = new UserDB();
            _eventDb = new EventDB();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("api/message/update")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update == null)
                return Ok();


            var commands = Bot.Commands;
            var message = update.Message;
            var client = new TelegramBotClient(AppSettings.Key);
            var chatId = message.Chat.Id;
            var text = message.Text;

            User user = await _userDb.GetUserByChatId(chatId);

            if (user == null)
            {
                commands.FirstOrDefault(c => c.Name == "/start")?.Execute(message, client);
                return Ok();
            }

            if (!user.IsAuthed)
            {
                commands.FirstOrDefault(c => c.Name == "/start" || c.Name == "Личный кабинет")?.Execute(message, client);
                return Ok();
            }

            switch (user.CurrentAction)
            {
                case Actions.None:
                    foreach (var command in commands)
                    {
                        if (command.Contains(message))
                        {
                            await command.Execute(message, client);
                            return Ok();
                        }
                    }
                    await client.SendTextMessageAsync(
                        chatId,
                        "Я не понимаю вас",
                        ParseMode.Html);

                    break;
                    
                case Actions.WaitingForEventCode:
                    bool isValid = await _eventDb.IsEventCodeValid(text);
                    if (isValid)
                    {
                        Event ev = await _userDb.Context.Events.FirstOrDefaultAsync(e => e.EventCode == text || e.AdminCode == text);
                        bool have = user.UserEvents.FirstOrDefault(ue => ue.EventId == ev.EventId) != null;
                        if (have)
                        {
                            await client.SendTextMessageAsync(
                                chatId,
                                "Вы уже присоединились к этому мероприятию",
                                ParseMode.Html);
                        }
                        else
                        {
                            await client.SendTextMessageAsync(
                                    chatId, 
                                $"Вы успешно присоединились к мероприятию \"{ev.Name}\"",
                                ParseMode.Html);
                            UserEvent userEvent = new UserEvent()
                            {
                                UserId = user.UserId,
                                EventId = ev.EventId
                            };
                            user.UserEvents.Add(userEvent);
                            user.CurrentEventId = ev.EventId;
                            _userDb.Context.Users.Update(user);
                            await _userDb.Context.SaveChangesAsync();
                        }
                        _userDb.ResetAction(chatId);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                            chatId, 
                            $"Неправильный код(",
                            ParseMode.Html);
                    }
                    break;
                
                case Actions.DeleteOrNot:
                    if (text == "Да")
                    {
                        _userDb.UserLogoff(chatId);
                        _userDb.ResetAction(chatId);
                        await client.SendTextMessageAsync(
                            chatId,
                            "Вы успешно прекратили пользоваться evectbot, для того чтобы начать заново напишите <em>/start</em>",
                            ParseMode.Html);
                    }
                    else if (text == "Нет")
                    {
                        _userDb.Context.Users.Remove(user);
                        await _userDb.Context.SaveChangesAsync();
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

                    break;
                case Actions.Profile:
                    if (text == "О мероприятии")
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            "мяу",
                            ParseMode.Html);
                    } else if (text == "Присоединиться к мероприятию")
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            "гав",
                            ParseMode.Html);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                            chatId,
                            "чот не то",
                            ParseMode.Html);
                    }
                    break;
            }

            return Ok();
        }
    }
}