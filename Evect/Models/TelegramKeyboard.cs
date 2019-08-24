using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace Evect.Models
{
    public class TelegramKeyboard
    {
        private List<List<KeyboardButton>> _buttons;

        private ReplyKeyboardMarkup _markup;
        public ReplyKeyboardMarkup Markup => GetKeyboard(_buttons, _isOneTime);
        private readonly bool _isOneTime;

        public TelegramKeyboard()
        {
            _buttons = new List<List<KeyboardButton>>();
            Clear();
        }

        public TelegramKeyboard(bool isOneTime) : this()
        {
            _isOneTime = isOneTime;
        }
        
        public TelegramKeyboard(IEnumerable<IEnumerable<string>> buttons, bool isOneTime = false) : this(isOneTime)
        {
            foreach (var enumerable in buttons)
            {
                AddRow(enumerable);
            }
        }

        public void AddRow(params string[] buttons)
        {
            if (buttons.Length != 0)
            {
                List<KeyboardButton> tempButtons = buttons
                    .Where(bb => bb != null)
                    .Select(b => new KeyboardButton(b))
                    .ToList();
                _buttons.Add(tempButtons);
            }
            else
            {
                throw new ArgumentException("'buttons' array has zero length");
            }
        }

        public void AddRow(IEnumerable<string> buttons)
        {
            if (buttons != null)
            {
                List<KeyboardButton> tempButtons = buttons.Select(b => new KeyboardButton(b)).ToList();
                _buttons.Add(tempButtons);
            }
        }

        private ReplyKeyboardMarkup GetKeyboard(List<List<KeyboardButton>> buttons, bool isOneTime)
        {
            if (buttons.Count == 0)
                throw new ArgumentException("You haven't added any row");
            
            if (buttons.Any(b => b.Count == 0))
                throw new ArgumentException("One of rows doesnt have buttons ");
            
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = buttons,
                ResizeKeyboard = true,
                OneTimeKeyboard = isOneTime
            };

            return keyboard;
        }

        public void Clear()
        {
            _buttons.Clear();
        }
        
        
    }
}