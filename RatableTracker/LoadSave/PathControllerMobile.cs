using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class PathControllerMobile : PathControllerWindows
    {
        public override string ApplicationDirectory()
        {
#if DEBUG
            return Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DEV");
#else
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
        }
    }
}
