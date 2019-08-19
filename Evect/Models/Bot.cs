using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evect.Models.Commands;
using Telegram.Bot;

namespace Evect.Models
{
    public static class Bot
    {
        private static TelegramBotClient _client;
        private static List<BaseCommand> _commandsList;

        public static IReadOnlyList<BaseCommand> Commands => _commandsList.AsReadOnly();
        
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_client != null)
            {
                return _client;
            }

            _commandsList = Utils.GetEnumerableOfType<BaseCommand>().ToList();
            
            
            _client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, "api/message/update");
//            var hook = AppSettings.Url;
            await _client.SetWebhookAsync(hook);

            return _client;
        }
    }
}