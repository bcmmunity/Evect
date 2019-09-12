using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evect.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public long TelegramId { get; set; }
        public int AnswerMark { get; set; }
        public string AnswerMessage { get; set; }
        public int QuestionId { get; set; }
        public DateTime Date { get; set; }
    }
}
