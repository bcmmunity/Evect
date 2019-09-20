using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evect.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
       // public int SurveyId { get; set; }
        public string Questions { get; set; }
        public int EventId { get; set; }
        public int Type { get; set; }//0-с сообщением,1- с отметкой
    }
}
