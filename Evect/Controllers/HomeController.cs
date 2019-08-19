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
    [Route("api/message/update")]
    public class HomeController : Controller
    {

        private ApplicationContext _db;
        
//        public HomeController(ApplicationContext db)
//        {
//            _db = db;
//        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null)
                return Ok();

            UserDB userDb = new UserDB();
            var commands = Bot.Commands;
            var message = update.Message;
            var client = new TelegramBotClient(AppSettings.Key);
            var chatId = message.Chat.Id;
            User user = await userDb.GetUserByChatId(chatId);

            if (user.CurrentAction != null && user.CurrentAction == Actions.WaitingForEventCode)
            {
                EventDB eventDb = new EventDB();
                if (await eventDb.IsEventCodeValid(message.Text))
                {
                    await client.SendTextMessageAsync(chatId, "все чотко", ParseMode.Html);

                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "неверно амиго", ParseMode.Html);

                }
            }
            else
            {
                foreach (var command in commands)
                {
                    if (command.Contains(message))
                    {
                        await command.Execute(message, client);
                        return Ok();
                    }
                }
            }
            
            
            
            
            

            await client.SendTextMessageAsync(message.Chat.Id, "Not a command", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            return Ok();
        }
        

      
    }
}