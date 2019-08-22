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

        public HomeController(ApplicationContext db)
        {
            _db = db;
            _userDb = new UserDB();
            _eventDb = new EventDB();

            _commandHadler = new CommandHandler();
            _actionHandler = new ActionHandler();
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
            var actions = Bot.ActionList;

            var message = update.Message;
            var client = new TelegramBotClient(AppSettings.Key);
            var chatId = message.Chat.Id;

            User user = await _userDb.GetUserByChatId(chatId);//получаем айди юзера и его самого из бд

            if (user == null)
            {
                foreach (var methodInfo in commands)
                {
                    var act = methodInfo.GetCustomAttribute<TelegramCommand>().StringCommand;
                    if (act == "/start")
                    {
                        methodInfo.Invoke(_commandHadler, new object[] {message, client});
                    }
                }

                return Ok();
            }

            if (!user.IsAuthed)
            {
                foreach (var methodInfo in commands)
                {
                    var act = methodInfo.GetCustomAttribute<TelegramCommand>().StringCommand;
                    if (act == "/start" || act == "Личный кабинет")
                    {
                        methodInfo.Invoke(_commandHadler, new object[] {message, client});
                    }
                }

                return Ok();
            }

            foreach (var methodInfo in actions)
            {
                var act = methodInfo.GetCustomAttribute<UserAction>().Action;
                if (act == user.CurrentAction)
                {
                    methodInfo.Invoke(_actionHandler, new object[] {message, client});
                }
            }

            return Ok();
        }
    }
}