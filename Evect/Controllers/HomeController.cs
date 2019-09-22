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

        private Dictionary<string, Func<ApplicationContext, Message, TelegramBotClient, Task>> _commands;
        private Dictionary<Actions, Func<ApplicationContext, Message, TelegramBotClient, Task>> _actions;
        private Dictionary<string, Func<ApplicationContext, CallbackQuery, TelegramBotClient, Task>> _callbacks;

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
                        if (update.CallbackQuery.Data.StartsWith(pair.Key))
                        {
                            await pair.Value(db, update.CallbackQuery, _client);
                            return Ok();
                        }
                    }
                    
                    
                } else if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var text = message.Text;

                    User user = await UserDB.GetUserByChatId(db, chatId); //получаем айди юзера и его самого из бд
                   

                    try
                    {
                        if (user == null)
                        {
                            try
                            {
                                if (text == "/start")
                                {
                                    await _commands[text](db, message, _client);
                                }
                                return Ok();
                            }
                            catch (KeyNotFoundException e)
                            {
                                Console.WriteLine("no method for this text");
                            }

                        }
                        else
                        {
                            if (!user.IsAuthed)
                            {

                                try
                                {
                                    if (text == "/start" || text == "Личный кабинет")
                                    {
                                        await _commands[text](db, message, _client);
                                    }
                                    return Ok();
                                }
                                catch (KeyNotFoundException e)
                                {
                                    Console.WriteLine("no method for this text");
                                }
                                
                            }
                            else
                            {
                                try
                                {
                                    if (text == "/start" || text == "/stop")
                                    {
                                        await _commands[text](db, message, _client);
                                        return Ok();
                                    }
                                }
                                catch (KeyNotFoundException e)
                                {
                                    Console.WriteLine("no method for this text");
                                }
                                
                                try
                                {
                                    await _actions[user.CurrentAction](db, message, _client);
                                    return Ok();
                                }
                                catch (KeyNotFoundException e)
                                {
                                    Console.WriteLine("no method for this action");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        await _client.SendTextMessageAsync(user.TelegramId, $"{e.Source} {e.Message}");
                    }
                    
                }
            }
            
            

            

            

            return Ok();
        }
    }
}