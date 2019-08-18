using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evect.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Evect.Controllers
{
    [Route("api/message/update")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            Console.WriteLine("\n\n\n\n\n\n");
            Console.WriteLine(update.Message.Text);
            Console.WriteLine("\n\n\n\n\n\n");
            
            
            
            return Ok();
        }
        

      
    }
}