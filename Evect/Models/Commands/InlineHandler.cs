using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evect.Models.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
        [InlineCallback("999-")]
        public async Task OnAnsweringOnQuestion(ApplicationContext context, CallbackQuery query,
            TelegramBotClient client)
        {
            int questionId = Convert.ToInt32(query.Data.Split('-')[1]);
            var chatId = query.From.Id;
            User user = context.Users.FirstOrDefault(n => n.TelegramId == chatId);
            user.PreviousAction = user.CurrentAction;
            user.CurrentQuestionId = questionId;
            await UserDB.ChangeUserAction(context, chatId, Actions.AnswerToSurvey);
            await client.SendTextMessageAsync(chatId, "Напишите,пожалуйста,свой ответ на этот вопрос");
            //  await client.EditMessageTextAsync(chatId, query.Message.MessageId, "Напишите,пожалуйста,свой ответ на этот вопрос");
        }

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
            await client.EditMessageTextAsync(chatID, query.Message.MessageId,
                "Спасибо за ваш ответ!\nВы можете продолжить ваши действия");
            User user = context.Users.FirstOrDefault(n => n.TelegramId == chatID);
            Actions action = user.PreviousAction;
            await UserDB.ChangeUserAction(context, chatID, action);
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
                builder.AppendLine($"*{us.FirstName}* {us.CompanyAndPosition}");
                builder.AppendLine();
                builder.AppendLine($"_Чем полезен_: {us.Utility}");
                builder.AppendLine();
                builder.AppendLine($"_О чем можно пообщаться_: {us.Communication}");

                string ch;

                if (user.Contacts.Any(e => e.AnotherUserId == us.TelegramId))
                {
                    ch = Utils.GetCheckmark();
                }
                else
                {
                    ch = "В книжку";
                }

                inline
                    .AddTextRow("Назад", ch, "Встреча", "Вперед")
                    .AddCallbackRow($"change-{changeId - 1}", $"contact-{us.TelegramId}", $"meet-{us.TelegramId}",
                        $"change-{changeId + 1}");

                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, builder.ToString());
                await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, inline.Markup);
            }
        }

        [InlineCallback("meet-")]
        public async Task OnMeet(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]);
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            InfoAboutUsers info = context.InfoAboutUsers.FirstOrDefault(N => N.EventId == user.CurrentEventId); //Liza
            info.AmountOfRequestsOfMettings++; //liza
            StringBuilder builder = new StringBuilder();
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            inline
                .AddTextRow("Отклонить", "Принять")
                .AddCallbackRow($"decline-{query.From.Id}", $"accept-{query.From.Id}");

            builder.AppendLine("Вам назначена встреча от:");
            builder.AppendLine($"{user.FirstName} {user.LastName}");

            await client.SendTextMessageAsync(userId, builder.ToString(), replyMarkup: inline.Markup);
        }

        [InlineCallback("contact-")]
        public async Task OnBook(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]);

            User user = await UserDB.GetUserByChatId(context, query.From.Id); // kim
            InfoAboutUsers info = context.InfoAboutUsers.FirstOrDefault(N => N.EventId == user.CurrentEventId); //Liza
            info.AmountOfRequestsOfContacts++;
            User toAdd = await UserDB.GetUserByChatId(context, userId); // roma

            StringBuilder builder = new StringBuilder();


            List<ContactsBook> contacts = user.Contacts;

            if (contacts.All(e => e.AnotherUserId != toAdd.TelegramId))
            {
                ContactsBook book = new ContactsBook()
                {
                    OwnerId = user.TelegramId,
                    AnotherUserId = toAdd.TelegramId
                };
                user.Contacts.Add(book);
                context.Update(user);
                context.SaveChanges();
                builder.AppendLine($"Пользователь {toAdd.FirstName} {toAdd.LastName} добавлен в записную книжку");

                await client.SendTextMessageAsync(user.TelegramId, builder.ToString());
            }
        }


        [InlineCallback("accept-")]
        public async Task OnAccept(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]); // roma

            User user = await UserDB.GetUserByChatId(context, query.From.Id); // kim

            User from = await UserDB.GetUserByChatId(context, userId);

            await client.SendTextMessageAsync(user.TelegramId,
                $"Встреча с {from.FirstName} {from.LastName} согласована");
            await client.SendTextMessageAsync(from.TelegramId,
                $"Встреча с {user.FirstName} {user.LastName} согласована");
        }

        [InlineCallback("decline-")]
        public async Task OnDecline(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]); // roma

            User user = await UserDB.GetUserByChatId(context, query.From.Id); // kim

            User from = await UserDB.GetUserByChatId(context, userId);

            await client.SendTextMessageAsync(user.TelegramId,
                $"Вы отменили встречу с {from.FirstName} {from.LastName}");
            await client.SendTextMessageAsync(from.TelegramId,
                $"К сожалению, {user.FirstName} {user.LastName} отказался от встречи");
        }

        [InlineCallback("prof-")]
        public async Task OnProfile(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]); // roma
            StringBuilder builder = new StringBuilder();
            User us = await UserDB.GetUserByChatId(context, userId);
            builder.AppendLine($"*{us.FirstName}* {us.CompanyAndPosition}");
            builder.AppendLine();
            builder.AppendLine($"_Чем полезен_: {us.Utility}");
            builder.AppendLine();
            builder.AppendLine($"_О чем можно пообщаться_: {us.Communication}");

            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
            inline
                .AddTextRow(Utils.GetCheckmark(), "Встреча")
                .AddCallbackRow($"contact-{us.TelegramId}", $"meet-{us.TelegramId}");

            await client.SendTextMessageAsync(query.From.Id, builder.ToString(), ParseMode.Markdown,
                replyMarkup: inline.Markup);
        }

        [InlineCallback("profpage-")]
        public async Task OnProfilePage(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            int page = Convert.ToInt32(query.Data.Split('-')[1]); // roma
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            StringBuilder builder = new StringBuilder();
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            List<ContactsBook> contacts = user.Contacts.Skip(4 * (page - 1)).Take(4).ToList();
            List<string> nums = new List<string>(4);
            List<string> ids = new List<string>(4);


            var tags = user.SearchingUserTags
                .Select(u => context.SearchingTags.FirstOrDefault(t => t.SearchingTagId == u.TagId)?.Name).ToList();

            builder.AppendLine("_Ваши теги_");
            builder.AppendLine($"`{string.Join(", ", tags)}`");
            builder.AppendLine();
            builder.AppendLine("*Ваши контакты*");
            builder.AppendLine();
            int i = 1;

            foreach (var contactsBook in contacts)
            {
                nums.Add(i.ToString());
                ids.Add($"prof-{contactsBook.AnotherUserId}");
                User another = await UserDB.GetUserByChatId(context, contactsBook.AnotherUserId);
                builder.AppendLine($"*{i}){another.FirstName} {another.LastName}* {another.CompanyAndPosition}");
                builder.AppendLine($"_Чем полезен_: {another.Utility}");
                builder.AppendLine($"_Контакт_: @{another.TelegramUserName}");
                builder.AppendLine();
                i++;
            }

            if (user.Contacts.Count > 4 * page)
            {
                if (page != 1)
                {
                    inline
                        .AddTextRow("Назад","Вперед")
                        .AddCallbackRow($"profpage-{page - 1}",$"profpage-{page + 1}");
                }
                else
                {
                    inline
                        .AddTextRow("Вперед")
                        .AddCallbackRow($"profpage-{page + 1}");
                }
                
            }
            else 
            {
                inline
                    .AddTextRow("Назад")
                    .AddCallbackRow($"profpage-{page - 1}");
            }

            inline
                .AddTextRow(nums.ToArray())
                .AddCallbackRow(ids.ToArray());

            await client.SendTextMessageAsync(query.From.Id, builder.ToString(), ParseMode.Markdown,
                replyMarkup: inline.Markup);
        }
    }
}