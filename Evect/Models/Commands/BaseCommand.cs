using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Evect.Models.Commands
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);
        
        public virtual bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
            {
                return false;
            }
            return message.Text.Contains(Name);
        }
        
    }
}