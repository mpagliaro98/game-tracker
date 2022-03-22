using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerMobile.ViewModels
{
    public class PopupListOption
    {
        public int? Value { get; set; }
        public string Text { get; set; }

        public PopupListOption(int? value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
