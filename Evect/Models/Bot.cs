using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Evect.Models.Commands;
using Telegram.Bot;

namespace Evect.Models
{
    public static class Bot
    {
        private static TelegramBotClient _client;
        private static List<MethodInfo> _commandsList;
        private static List<MethodInfo> _actionList;
        public static IReadOnlyList<MethodInfo> Commands => _commandsList.AsReadOnly();
        public static IReadOnlyList<MethodInfo> ActionList => _actionList.AsReadOnly();
        
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_client != null)
            {
                return _client;
            }
            
            Assembly assembly = Assembly.GetAssembly(typeof(Actions));

            
            _commandsList = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(TelegramCommand), false).Length > 0)
                .ToList();
            
            _actionList = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(UserAction), false).Length > 0)
                .ToList();
            
            _client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, "api/message/update");
//            var hook = AppSettings.Url;
            await _client.SetWebhookAsync(hook);

            return _client;
        }
    }
}