using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Evect.Models.Commands;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Evect.Models
{
    public static class Bot
    {
        private static TelegramBotClient _client;


        private static Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, string > _commandsList = new Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, string>();
        private static Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, Actions> _actionList =  new Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, Actions>();
        
        public static Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, string> Commands => _commandsList;
        public static Dictionary<Func<ApplicationContext, Message, TelegramBotClient, Task>, Actions > ActionList => _actionList;
        

        
        
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_client != null)
            {
                return _client;
            }
            
            
            Assembly assembly = Assembly.GetAssembly(typeof(Actions));

            
            var commandsMethodInfo = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(TelegramCommand), false).Length > 0)
                .ToList();
            
            CommandHandler _commandHandler = new CommandHandler();
            ActionHandler _actionHandler = new ActionHandler();
            
            foreach (var methodInfo in commandsMethodInfo)
            {
                Func<ApplicationContext, Message, TelegramBotClient, Task> a = 
                    (Func<ApplicationContext, Message, TelegramBotClient, Task>) Delegate.CreateDelegate(typeof(Func<ApplicationContext, Message, TelegramBotClient, Task>), _commandHandler, methodInfo);

                string c = methodInfo.GetCustomAttribute<TelegramCommand>().StringCommand;
                _commandsList.Add(a, c);
            }
            
            var actionMethodInfo = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(UserAction), false).Length > 0)
                .ToList();
           
            
            foreach (var methodInfo in actionMethodInfo)
            {
                Func<ApplicationContext, Message, TelegramBotClient, Task> a = 
                                    (Func<ApplicationContext, Message, TelegramBotClient, Task>) Delegate.CreateDelegate(typeof(Func<ApplicationContext, Message, TelegramBotClient, Task>),_actionHandler ,methodInfo);
                
                Actions act = methodInfo.GetCustomAttribute<UserAction>().Action;
                _actionList.Add(a, act);
            }

            
            _client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, "api/message/update");
            await _client.SetWebhookAsync(hook);

            return _client;
        }
    }
}