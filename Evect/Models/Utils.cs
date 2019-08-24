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
        
        public  async Task SendEmailAsync(string email,string subject,string message)//эмейл получателя,тема письма,текст письма
        {
            var emailMessage = new MimeMessage();//создет объект отправляемого сообщения
            emailMessage.From.Add(new MailboxAddress("Администрация бота", "info@diffind.com"));//определяется отправитель
            emailMessage.To.Add(new MailboxAddress(email));//коллекция получателей
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)//тело сообщения
            {
                Text = message
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())//непосредственно само отправление сообщения
            {
                await client.ConnectAsync("wpl19.hosting.reg.ru", 587,false);//подключение к серверу
                await client.AuthenticateAsync("info@diffind.com", "Diffind123!");//аутенфикация
                await client.SendAsync(emailMessage);//отправка сообщения
                await client.DisconnectAsync(true);//отключение
            }
        }
        
        
        
        
    }
}