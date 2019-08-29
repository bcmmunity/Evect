using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Evect.Models.Commands
{
    public class InlineHandler
    {
        [InlineCallback("123", "1222")]
        public async Task OnTest(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
            inline
                .AddTextRow("567")
                .AddCallbackRow("56");
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, "meow");
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, replyMarkup: inline.Markup);
        }
    }
}