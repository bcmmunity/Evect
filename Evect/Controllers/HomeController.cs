using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evect.Models;
using Evect.Models.Commands;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using User = Evect.Models.User;

namespace Evect.Controllers
{
    public class HomeController : Controller
    {
        private TelegramBotClient _client;
        
        private UserDB _userDb;
        private EventDB _eventDb;

        private CommandHandler _commandHadler;
        private ActionHandler _actionHandler;

        private Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, string> _commands;
        private Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, Actions> _actions;
        private Dictionary<Func<ApplicationContext, CallbackQuery, TelegramBotClient, Task>, string> _callbacks;

        public HomeController(ApplicationContext db)
        {
            _eventDb = new EventDB();

            _commandHadler = new CommandHandler();
            _actionHandler = new ActionHandler();

            _commands = Bot.Commands;
            _actions = Bot.ActionList;
            _callbacks = Bot.CallbackList;
            
            _client = new TelegramBotClient(AppSettings.Key);
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

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                
                if (update.Type == UpdateType.CallbackQuery)
                {
                    foreach (var pair in _callbacks)
                    {
                        if (update.CallbackQuery.Data.StartsWith(pair.Value))
                        {
                            await pair.Key(db, update.CallbackQuery, _client);
                            return Ok();
                        }
                    }
                } else if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var text = message.Text;

                    User user = await UserDB.GetUserByChatId(db, chatId); //получаем айди юзера и его самого из бд
                    if (user == null)
                    {
                        foreach (var pair in _commands)
                        {
                            if (text == "/start" && pair.Value == "/start")
                            {
                                await pair.Key(db, message, _client);
                                return Ok();
                            }
                        }

                    }
                    else
                    {
                        if (!user.IsAuthed)
                        {
                            foreach (var pair in _commands)
                            {
                                if ((text == "/start" || text == "Личный кабинет") &&
                                    (pair.Value == text))
                                {
                                    await pair.Key(db, message, _client);
                                    return Ok();
                                }
                            }
                        }
                        else
                        {
                            foreach (var pair in _commands)
                            {
                                if ((text == "/start" || text == "/stop") &&
                                    (pair.Value == text))
                                {
                                    await pair.Key(db, message, _client);
                                    return Ok();
                                }
                            }
                            foreach (var pair in _actions)
                            {
                                if (pair.Value == user.CurrentAction)
                                {
                                    await pair.Key(db, message, _client);
                                    return Ok();
                                }
                            }
                        }
                    }
                }
            }
            
            

            

            

            return Ok();
        }
    }
}