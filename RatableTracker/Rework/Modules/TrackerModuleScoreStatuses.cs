using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleScoreStatuses : TrackerModuleScores
    {
        private readonly StatusExtensionModule _statusExtension;
        public StatusExtensionModule StatusExtension { get { return _statusExtension; } }

        public TrackerModuleScoreStatuses(ILoadSaveMethod loadSave) : base(loadSave)
        {
            _statusExtension = new StatusExtensionModule(this);
        }
    }
}
