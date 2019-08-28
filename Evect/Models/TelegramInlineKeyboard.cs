using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace Evect.Models
{
    public class TelegramInlineKeyboard
    {
        private readonly List<List<string>> _textRows;
        private readonly List<List<string>> _callbackRow;
        
        public InlineKeyboardMarkup Markup => GetKeyboard(_textRows, _callbackRow);

        public TelegramInlineKeyboard()
        {
            _textRows = new List<List<string>>();
            _callbackRow = new List<List<string>>();
        }

        public TelegramInlineKeyboard AddTextRow(params string[] data)
        {
            if (_textRows.Count < _callbackRow.Count)
            {
                if (_callbackRow.Last()?.Count != data.Length)
                {
                    throw new ArgumentException("Count of text data must equals to count of callback data");
                }
            }
            _textRows.Add(data.ToList());
            return this;
        }

        public TelegramInlineKeyboard AddCallbackRow(params string[] data)
        {
            if (_callbackRow.Count < _textRows.Count)
            {
                if (_textRows.Last()?.Count != data.Length)
                {
                    throw new ArgumentException("Count of callback data must equals to count of text data");
                }
            }
            _callbackRow.Add(data.ToList());
            return this;
        }

        public InlineKeyboardMarkup GetKeyboard(List<List<string>> textRows, List<List<string>> callbackRows)
        {
            List<List<InlineKeyboardButton>> buttons = textRows
                .Zip(callbackRows, (list, list1) => list
                    .Zip(list1, (s, s1) =>  new InlineKeyboardButton() {Text = s, CallbackData = s1})
                    .ToList())
                .ToList();
            return new InlineKeyboardMarkup(buttons);
        }
    }
}