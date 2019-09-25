using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
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
            await UserDB.ChangeUserAction(context, chatID, Actions.AdminMode);
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
            int left = changeId;
            
            
            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            StringBuilder builder = new StringBuilder();
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();
            List<User> usersWithTags = context.Users
                .Include(u => u.UserTags)
                .ThenInclude(ut => ut.Tag)
                .Where(e => e.UserTags.Any(ut =>
                                user.SearchingUserTags.FirstOrDefault(t => t.TagId == ut.TagId) != null) &&
                            e.CurrentEventId == user.CurrentEventId).ToList();

            List<User> usersWithoutTags = context.Users
                .Include(u => u.UserTags)
                .ThenInclude(ut => ut.Tag)
                .Where(e =>
                    e.UserTags.All(ut =>
                        user.SearchingUserTags.FirstOrDefault(t => t.TagId == ut.TagId) == null) &&
                    e.CurrentEventId == user.CurrentEventId).ToList();

            usersWithTags.AddRange(usersWithoutTags);

    
            
            if (changeId == 0)
            {
                changeId = usersWithTags.Count;
                left = changeId;
            }        
            
            if (changeId > 0 && changeId <= usersWithTags.Count)
            {
                User us = usersWithTags[changeId - 1];
                builder.AppendLine($"{us.FirstName} {us.CompanyAndPosition}");
                builder.AppendLine();
                builder.AppendLine($"_Теги_: {string.Join(", ", us.UserTags.Select(e => e.Tag.Name))}");
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
                
                if (changeId + 1 > usersWithTags.Count)
                {
                    changeId = 0;
                    left = 2;
                }
                
                if (changeId - 1 < 0)
                {
                    left = 2;
                }

                
                

                
                
                
                
    
                inline
                    .AddTextRow("⬅️", ch, "Встреча", "➡️")
                    .AddCallbackRow($"change-{left - 1}", $"contact-{us.TelegramId}", $"meet-{us.TelegramId}",
                        $"change-{changeId + 1}");


                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, builder.ToString(),
                    ParseMode.Markdown);
                await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, inline.Markup);
            }
        }

        [InlineCallback("meet-")]
        public async Task OnMeet(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt64(query.Data.Split('-')[1]);
            User user = await UserDB.GetUserByChatId(context, query.From.Id);

            StringBuilder builder = new StringBuilder();
            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            inline
                .AddTextRow("Отклонить", "Принять")
                .AddCallbackRow($"decline-{query.From.Id}", $"accept-{query.From.Id}");

            builder.AppendLine("Вам назначена встреча от:");
            builder.AppendLine($"{user.FirstName} {user.LastName}");

            await client.SendTextMessageAsync(userId, builder.ToString(), ParseMode.Markdown,
                replyMarkup: inline.Markup);
        }

        [InlineCallback("contact-")]
        public async Task OnBook(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt64(query.Data.Split('-')[1]);

            User user = await UserDB.GetUserByChatId(context, query.From.Id); // kim
            InfoAboutUsers info = context.InfoAboutUsers.FirstOrDefault(N => N.EventId == user.CurrentEventId); //Liza
            info.AmountOfRequestsOfContacts++;
            context.Update(info);
            context.SaveChanges();
            User toAdd = await UserDB.GetUserByChatId(context, userId); // roma

            StringBuilder builder = new StringBuilder();

            TelegramInlineKeyboard inline = new TelegramInlineKeyboard();

            List<ContactsBook> contacts = user.Contacts;

            if (contacts.All(e => e.AnotherUserId != toAdd.TelegramId))
            {
                ContactsBook book = new ContactsBook()
                {
                    OwnerId = user.TelegramId,
                    AnotherUserId = toAdd.TelegramId,
                    IsAccepted = false
                };
                user.Contacts.Add(book);
                context.Update(user);
                context.SaveChanges();

                await client.SendTextMessageAsync(user.TelegramId, builder.ToString(), ParseMode.Markdown);

                inline
                    .AddTextRow("Да", "Нет")
                    .AddCallbackRow($"contacts_accept-{user.TelegramId}",$"contacts_decline-{user.TelegramId}");

                await client.SendTextMessageAsync(toAdd.TelegramId,
                    $"{user.FirstName} сделал запрос на добавление контактов, потвердить?", ParseMode.Markdown, replyMarkup: inline.Markup);

            }
        }

        [InlineCallback("contacts_accept-")]
        public async Task OnContactsAccept(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt64(query.Data.Split('-')[1]);

            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            User anotherUser = await UserDB.GetUserByChatId(context, userId);

            ContactsBook contact = user.Contacts.FirstOrDefault(c => c.OwnerId == query.From.Id && c.AnotherUserId == userId);

            if (contact != null)
            {
                contact.IsAccepted = true;

                context.ContactsBooks.Update(contact);
                context.SaveChanges();
                await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, InlineKeyboardMarkup.Empty());
            
                await client.SendTextMessageAsync(query.From.Id, $"Вы дали доступ к своим данным пользователю {anotherUser.FirstName}",
                    ParseMode.Markdown);
            
                await client.SendTextMessageAsync(userId, $"{user.FirstName} потвердил запрос на данные",
                    ParseMode.Markdown);
            }
        }
        
        [InlineCallback("contacts_decline-")]
        public async Task OnContactsDecline(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt64(query.Data.Split('-')[1]);

            User user = await UserDB.GetUserByChatId(context, query.From.Id);
            User anotherUser = await UserDB.GetUserByChatId(context, userId);
            
            await client.EditMessageReplyMarkupAsync(query.From.Id, query.Message.MessageId, InlineKeyboardMarkup.Empty());
            
            await client.SendTextMessageAsync(query.From.Id, $"Вы отказали в доступе к контакту",
                ParseMode.Markdown);
            
            await client.SendTextMessageAsync(userId, $"{user.FirstName} отменил запрос на данные",
                ParseMode.Markdown);
            
        }



        [InlineCallback("accept-")]
        public async Task OnAccept(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]); // roma
            User user = await UserDB.GetUserByChatId(context, query.From.Id); // ki
            InfoAboutUsers info = context.InfoAboutUsers.FirstOrDefault(n => n.EventId == user.CurrentEventId);
            info.AmountCompletedMeetings++;
            context.Update(info);
            context.SaveChanges();
            User from = await UserDB.GetUserByChatId(context, userId);

            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);

            await client.SendTextMessageAsync(user.TelegramId,
                $"Встреча с [{from.FirstName} {from.LastName}](tg://user?id={from.TelegramId}) согласована 👐, напишите ему для уточнения времени и места", ParseMode.Markdown);
            await client.SendTextMessageAsync(from.TelegramId,
                $"Встреча с [{user.FirstName} {user.LastName}](tg://user?id={user.TelegramId}) согласована 👐, напишите ему для уточнения времени и места", ParseMode.Markdown);
        }

        [InlineCallback("decline-")]
        public async Task OnDecline(ApplicationContext context, CallbackQuery query, TelegramBotClient client)
        {
            long userId = Convert.ToInt32(query.Data.Split('-')[1]); // roma

            User user = await UserDB.GetUserByChatId(context, query.From.Id); // kim

            User from = await UserDB.GetUserByChatId(context, userId);

            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);

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
            builder.AppendLine($"{us.FirstName} {us.CompanyAndPosition}");
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
                builder.AppendLine($"*{i})*{another.FirstName} {another.LastName} {another.CompanyAndPosition}");
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
                        .AddTextRow("⬅️", "➡️")
                        .AddCallbackRow($"profpage-{page - 1}", $"profpage-{page + 1}");
                }
                else
                {
                    inline
                        .AddTextRow("➡️")
                        .AddCallbackRow($"profpage-{page + 1}");
                }
            }
            else
            {
                inline
                    .AddTextRow("⬅️")
                    .AddCallbackRow($"profpage-{page - 1}");
            }

            inline
                .AddTextRow(nums.ToArray())
                .AddCallbackRow(ids.ToArray());

            await client.SendTextMessageAsync(query.From.Id, builder.ToString(), parseMode: ParseMode.Markdown,
                replyMarkup: inline.Markup);
        }
        
        
    }
}