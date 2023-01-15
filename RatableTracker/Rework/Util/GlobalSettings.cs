using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public static class GlobalSettings
    {
        public static bool Autosave { get; set; } = true;

        public static RatableTrackerFactory FactoryInstance { get; set; } = new RatableTrackerFactory();

        public static IPathController PathController { get; set; } = new PathControllerWindows();
    }
}
