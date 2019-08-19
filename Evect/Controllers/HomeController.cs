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

        public HomeController(ApplicationContext db)
        {
            _db = db;
            _userDb = new UserDB();
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
                commands.FirstOrDefault(c => c.Name == "/start")?.Execute(message, client);
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
            }

            return Ok();
        }
    }
}