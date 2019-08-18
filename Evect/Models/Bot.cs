using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Evect.Models
{
    public static class Bot
    {
        private static TelegramBotClient _client;
        private static List<Command> _commandsList;

        public static IReadOnlyList<Command> Commands => _commandsList;
        
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_client != null)
            {
                return _client;
            }

            _commandsList = new List<Command>();

            _client = new TelegramBotClient(AppSettings.Key);
//            var hook = string.Format(AppSettings.Url, "api/message/update");
            var hook = AppSettings.Url;
            await _client.SetWebhookAsync(hook);

            return _client;
        }
    }
}