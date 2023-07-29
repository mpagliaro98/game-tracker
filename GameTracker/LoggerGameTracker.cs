using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            try
            {
                var gtAssembly = Assembly.GetExecutingAssembly();
                var frames = new StackTrace().GetFrames();
                var uiAssembly = Assembly.GetEntryAssembly() ?? (frames.Length >= 4 ? frames[3].GetMethod().Module.Assembly : null);
                Log("\nGAME TRACKER - " + gtAssembly.FullName + "\nUI - " + (uiAssembly == null ? "UNABLE TO DETERMINE" : uiAssembly.FullName));
            }
            catch (Exception ex)
            {
                Log("Unable to log Game Tracker version info due to an error: " + ex.GetType().Name + " - " + ex.Message);
            }
        }
    }
}
