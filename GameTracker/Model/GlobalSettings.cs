using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public static partial class GlobalSettings
    {
        private static bool autosave = true;
        public static bool Autosave
        {
            get { return autosave; }
            set { autosave = value; }
        }
    }
}
