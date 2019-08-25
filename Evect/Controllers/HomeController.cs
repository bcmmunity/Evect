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

        private CommandHandler _commandHadler;
        private ActionHandler _actionHandler;

        private Dictionary<Action<Message, TelegramBotClient>, string > _commands;
        private Dictionary<Action<Message, TelegramBotClient>, Actions > _actions;
        
        public HomeController(ApplicationContext db)
        {
            _db = db;
            _userDb = new UserDB();
            _eventDb = new EventDB();

            _commandHadler = new CommandHandler();
            _actionHandler = new ActionHandler();
        
            _commands = Bot.Commands;
            _actions = Bot.ActionList;
                
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
            
            
            var message = update.Message;
            var client = new TelegramBotClient(AppSettings.Key);
            var chatId = message.Chat.Id;

            User user = await _userDb.GetUserByChatId(chatId);//получаем айди юзера и его самого из бд

            if (user == null)
            {
                foreach (var pair in _commands)
                {
                    if (pair.Value == "/start")
                    {
                        pair.Key(message, client);
                    } 
                }

                return Ok();
            }

            if (!user.IsAuthed)
            {
                foreach (var pair in _commands)
                {
                    if (pair.Value == "/start" || pair.Value == "Личный кабинет")
                    {
                        pair.Key(message, client);
                    } 
                }
                
                return Ok();
            }
            
            foreach (var pair in _actions)
            {
                if (pair.Value == user.CurrentAction)
                {
                    pair.Key(message, client);
                }
            }

            

            return Ok();
        }
    }
}