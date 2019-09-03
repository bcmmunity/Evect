using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId,
                replyMarkup: inline.Markup);
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
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId,
                replyMarkup: inline.Markup);
        }

        [InlineCallback("change-")]
        public async Task OnChange(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            int changeId = Convert.ToInt32(query.Data.Split('-')[1]);
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            StringBuilder builder = new StringBuilder();
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
            List<User> usersWithTags = context.Users.Where(e =>
                e.UserTags.Any(ut =>
                    user.UserTags.FirstOrDefault(t => t.TagId == ut.TagId) != null) &&
                e.CurrentEventId == user.CurrentEventId).ToList();

            List<User> usersWithoutTags = context.Users.Where(e =>
                e.UserTags.Any(ut =>
                    user.UserTags.FirstOrDefault(t => t.TagId == ut.TagId) == null) &&
                e.CurrentEventId == user.CurrentEventId).ToList();
            
            usersWithTags.AddRange(usersWithoutTags);
            
            
            
            if (changeId > 0 && changeId <= usersWithTags.Count)
            {
                User us = usersWithTags[changeId - 1];
                builder.AppendLine($"{us.FirstName} {us.LastName}");
                builder.AppendLine(us.CompanyAndPosition);
                builder.AppendLine();
                builder.AppendLine("Чем полезен");
                builder.AppendLine(us.Utility);
                builder.AppendLine();
                builder.AppendLine("О чем можно пообщаться");
                builder.AppendLine(us.Communication);
                    
                inline
                    .AddTextRow("Назад","В книжку","Встреча", "Вперед")
                    .AddCallbackRow($"change-{changeId - 1}",$"contact-{us.UserId}",$"meet-{us.UserId}",$"change-{changeId + 1}");

                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, builder.ToString());
                await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, inline.Markup);
            }
            
            
        }
    }
}