using System;

namespace Evect.Models.Commands
{

    [AttributeUsage(AttributeTargets.Method)]
    public class TelegramCommand : Attribute
    {
        public string StringCommand { get; set; }

        public TelegramCommand(string command)
        {
            StringCommand = command;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]//Этот метод применим только к методам
    public class UserAction : Attribute
    {
        public Actions Action{ get; set; }

        public UserAction(Actions action)
        {
            Action = action;
        }
    }
    
}