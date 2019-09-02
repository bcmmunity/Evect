using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Evect.Models.Commands
{
    public class InlineHandler
    {
//        [InlineCallback("123)]
//        public async Task OnTest(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
//        {
//            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
//            inline
//                .AddTextRow("567")
//                .AddCallbackRow("56");
//            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, "meow");
//            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, replyMarkup: inline.Markup);
//        }


        [InlineCallback("tag-")]
        public async Task OnAddingTag(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            int tagId = Convert.ToInt32(query.Data.Split('-')[1]);
            int parentTagId = context.Tags.Where(t => t.TagId == tagId).Select(e => e.ParentTagID).First();
            List<Tag> childTags = context.Tags.Where(x => x.Level == 2 && x.ParentTagID == parentTagId).ToList();
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            bool added = await UserDB.AddDeleteTag(context, query.From.Id, tagId);

            string ch = "";
            bool ex;
            foreach (var tag in childTags)
            {
                if (tag.TagId != tagId)
                {
                    ex = user.UserTags.FirstOrDefault(e => e.TagId == tag.TagId) != null;
                }
                else
                {
                    ex = added;
                }

                if (ex)
                {
                    ch = Utils.GetCheckmark();
                }
                else
                {
                    ch = "";
                }
                inline.AddTextRow($"{tag.Name} {ch}").AddCallbackRow($"tag-{tag.TagId}");
            }

            
//            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, "meow");
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, replyMarkup: inline.Markup);
        }
        
        [InlineCallback("searchtag-")]
        public async Task OnAddingSearchTag(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            int tagId = Convert.ToInt32(query.Data.Split('-')[1]);
            int parentTagId = context.Tags.Where(t => t.TagId == tagId).Select(e => e.ParentTagID).First();
            List<Tag> childTags = context.Tags.Where(x => x.Level == 2 && x.ParentTagID == parentTagId).ToList();
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            bool added = await UserDB.AddDeleteSearchTag(context, query.From.Id, tagId);

            string ch = "";
            bool ex;
            foreach (var tag in childTags)
            {
                if (tag.TagId != tagId)
                {
                    ex = user.SearchingUserTags.FirstOrDefault(e => e.TagId == tag.TagId) != null;
                }
                else
                {
                    ex = added;
                }

                if (ex)
                {
                    ch = Utils.GetCheckmark();
                }
                else
                {
                    ch = "";
                }
                inline.AddTextRow($"{tag.Name} {ch}").AddCallbackRow($"searchtag-{tag.TagId}");
            }

            
//            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, "meow");
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, replyMarkup: inline.Markup);
        }
        
    }
}