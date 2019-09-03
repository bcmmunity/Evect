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
        /* [InlineCallback("999-")]
         public async Task OnAnsweringOnQuestion(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
         {
             int questionId=Convert.ToInt32(query.Data.Split('-')[1]);
             var chatId = query.From.Id;
             User user = context.Users.FirstOrDefault(n => n.TelegramId == chatId);
             user.PreviousAction = user.CurrentAction;
             await UserDB.ChangeUserAction(context, chatId, Actions.AnswerToSurvey);
            await UserDB.ChangeUserAction(context, Actions.AnswerToSurvey);

             await client.SendTextMessageAsync(chatId, "Напишите,пожалуйста,свой ответ на этот вопрос");
           //  await client.EditMessageTextAsync(chatId, query.Message.MessageId, "Напишите,пожалуйста,свой ответ на этот вопрос");
          }*/
        [InlineCallback("990-")]
        public async Task OnAnsweringOnSurvey(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            int mark = Convert.ToInt32(query.Data.Split('-')[1]);
            int questionId = Convert.ToInt32(query.Data.Split('-')[2]);
            var chatID = query.From.Id;
            Answer answer = new Answer();
            answer.AnswerMark = mark;
            answer.TelegramId = chatID;
            answer.QuestionId = questionId;
            context.Answers.Add(answer);
            context.SaveChanges();
            TelegramInlineKeyboard inlineKeyboard = new TelegramInlineKeyboard();
            await client.EditMessageTextAsync(chatID, query.Message.MessageId, "Спасибо за ваш ответ!");


        }
   


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
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, replyMarkup: inline.Markup);
        }
        
    }
}