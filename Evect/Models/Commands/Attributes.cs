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
    
    [AttributeUsage(AttributeTargets.Method)]
    public class UserAction : Attribute
    {
        public Actions Action{ get; set; }

        public UserAction(Actions action)
        {
            Action = action;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class InlineCallback : Attribute
    {
        public string Callbacks { get; set; }

        public InlineCallback(string callbacks)
        {
            Callbacks = callbacks;
        }
        
        
    }
    
}