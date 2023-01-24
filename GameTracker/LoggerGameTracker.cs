using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class LoggerGameTracker : LoggerThreaded
    {
        public LoggerGameTracker(IFileHandler fileHandler) : base(fileHandler) { }

        protected override void LogSystemInfoOnLoggerStart()
        {
            base.LogSystemInfoOnLoggerStart();
            Log("GAME TRACKER VERSION - " + Util.GameTrackerVersion.ToString());
        }
    }
}
