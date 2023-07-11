using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Model
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
