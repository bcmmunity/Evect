using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using Telegram.Bot.Types.Enums;

namespace Evect.Models
{
    public static class Utils
    {

        public static bool IsEmailValid(string email)
        {
            
            if (string.IsNullOrWhiteSpace(email)) return false;

            // почта должна содержать 1 и только 1 собачу
            var atCount = email.Count(c => c == '@');
            if (atCount != 1) return false;

            // должна содержать точку
            if (!email.Contains(".")) return false;

            // @ собачка должна стоять раньше точки и разница индексов должна быть больше 1
            var indexOfAt = email.IndexOf("@", StringComparison.Ordinal);
            var lastIndexOfPeriod = email.LastIndexOf(".", StringComparison.Ordinal);
            var atBeforeLastPeriod = lastIndexOfPeriod > indexOfAt && lastIndexOfPeriod - indexOfAt > 1;
            if (!atBeforeLastPeriod) return false;

            if (lastIndexOfPeriod == email.Length - 1) return false;

            if (!Char.IsLetterOrDigit(email[email.Length - 1])) return false;

            try
            {
                MailAddress m = new MailAddress(email);
                return m.Address == email;
            }
            catch (FormatException ex)
            {
                return false;
            }
        }
        
        public static async Task SendEmailAsync(string email,string subject,string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация бота", "info@diffind.com"));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = message
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("wpl19.hosting.reg.ru", 587,false);
                await client.AuthenticateAsync("info@diffind.com", "Diffind123!");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public static string GenerateRandomCode()
        {
            Random random = new Random();
            return random.Next(1000, 10000).ToString();
        }
        public static string GenerateNewCode(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                //Генерируем число являющееся латинским символом в юникоде
                ch = Convert.ToChar(random.Next(0, 26) + 65);
                //Конструируем строку со случайно сгенерированными символами
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string GetCheckmark(int type = 1)
        {
            switch (type)
            {
                case 1:
                    return "✅";
                case 2:
                    return "✔";
                case 3:
                    return "☑";
                case 4:
                    return "✓";
                default:
                    return "✅";
            }
        }

        /// <summary>
        /// Разбивает изначальный список на энное количество списков опреленной длины
        /// </summary>
        /// <param name="width">длина списка </param>
        /// <param name="list">изначальный список</param>
        public static List<List<T>> SplitList<T>(int width, List<T> list)
        {
            List<List<T>> splited = new List<List<T>>();
            if (list.Count <= width)
            {
                splited.Add(list);
                return splited;
            }

            int numberOfLists = list.Count / width;

            for (int i = 0; i < numberOfLists; i++) 
            { 
                List<T> newList = list.Skip(i * width).Take(width).ToList();
                splited.Add(newList); 
            }
            
            return splited;


        }

        public static string Correct(this string str)
        {
            if (str.Contains('*') || str.Contains('_') || str.Contains('`'))
            {
                str = str.Replace("*", @"\*");
                str = str.Replace("_", @"\_");
                str = str.Replace("`", @"\`");
            }

            return str;
        }
        
        

        public static TelegramKeyboard CommonKeyboards(Actions actions)
        {
            TelegramKeyboard keyboard = new TelegramKeyboard();
            switch (actions)
            {
                case Actions.AdminMode:
                    {
                        keyboard.AddRow("Об ивенте");
                        keyboard.AddRow("Информация о пользователях");
                        keyboard.AddRow("Создать опрос");
                        keyboard.AddRow("Создать оповещение");
                        keyboard.AddRow("Войти как обычный участник");
                    }
                    break;
                case Actions.Profile:
                    {
                        keyboard.AddRow("О мероприятии", "Присоединиться к мероприятию");
                        keyboard.AddRow("Режим нетворкинга");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Все мероприятия");
                    }
                    break;
                case Actions.GetInformationAboutTheEvent:
                    {
                        keyboard.AddRow("Добавить новую статью");
                        keyboard.AddRow("Редактировать");
                        keyboard.AddRow("Назад");
                    }
                    break;
                case Actions.InformationAboutUsers:
                    {
                        keyboard.AddRow("Количество пользователей");
                        keyboard.AddRow("Количество активаций режима общения");
                        keyboard.AddRow("Число запросов контактов");
                        keyboard.AddRow("Число запросов встреч");
                        keyboard.AddRow("Назад");
                    }
                    break;
                case Actions.CreateNotification:
                    {
                        
                        keyboard.AddRow("Назад");
                    }
                    break;
                case Actions.CreateSurvey:
                    {
                        keyboard.AddRow("Назад");
                        keyboard.AddRow("Опрос с развернутой обратной связью");
                        keyboard.AddRow("Опрос с оценкой");
                    }
                    break;
                case Actions.MyProfileEditing:
                    {
                        keyboard.AddRow("Имя и фамилия");
                        keyboard.AddRow("Почта");
                        keyboard.AddRow("Работа и должность");
                        keyboard.AddRow("Полезность");
                        keyboard.AddRow("О чем пообщаться");
                        keyboard.AddRow("Вернуться в мой профиль");
                    }
                    break;
                case Actions.NetworkingMenu:
                    {
                        keyboard.AddRow("Мой профиль");
                        keyboard.AddRow("Записная книжка");
                        keyboard.AddRow("Общение");
                        keyboard.AddRow("Вернуться на главную");
                    }
                    break;
                case Actions.SearchingParentTag:
                    {

                        keyboard.AddRow("Редактировать профиль");
                        keyboard.AddRow("Изменить теги");
                        keyboard.AddRow("Вернуться в режим общения");
                    }
                    break;
                case Actions.TagsEditing:
                    {

                        keyboard.AddRow("Теги поиска");
                        keyboard.AddRow("Личные теги");
                        keyboard.AddRow("Вернуться в мой профиль");
                    }
                    break;
            }
            return keyboard;
        }
    }
}