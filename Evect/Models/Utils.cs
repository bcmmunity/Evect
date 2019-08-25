using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using MimeKit;

namespace Evect.Models
{
    public class Utils
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constuctorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constuctorArgs));
            }

            return objects;
        }

        public static bool IsEmailValid(string email)
        {
            
            if (string.IsNullOrWhiteSpace(email)) return false;

            // MUST CONTAIN ONE AND ONLY ONE @
            var atCount = email.Count(c => c == '@');
            if (atCount != 1) return false;

            // MUST CONTAIN PERIOD
            if (!email.Contains(".")) return false;

            // @ MUST OCCUR BEFORE LAST PERIOD
            var indexOfAt = email.IndexOf("@", StringComparison.Ordinal);
            var lastIndexOfPeriod = email.LastIndexOf(".", StringComparison.Ordinal);
            var atBeforeLastPeriod = lastIndexOfPeriod > indexOfAt && lastIndexOfPeriod - indexOfAt > 1;
            if (!atBeforeLastPeriod) return false;

            if (lastIndexOfPeriod == email.Length - 1) return false;

            if (!Char.IsLetter(email[email.Length - 1])) return false;

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

        public static List<List<T>> SplitList<T>(int width, List<T> list)
        {
            List<List<T>> splited = new List<List<T>>();
            if (list.Count <= width)
            {
                splited.Add(list);
                return splited;
            }

            int numberOfLists = list.Count / width;

            for (int i = 0; i <= numberOfLists; i++) 
            { 
                List<T> newList = list.Skip(i * width).Take(width).ToList();
                splited.Add(newList); 
            }
            
            return splited;


        }
        
        
    }
}