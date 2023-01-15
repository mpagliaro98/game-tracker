using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleStatuses : TrackerModule
    {
        private readonly StatusExtensionModule _statusExtension;
        public StatusExtensionModule StatusExtension { get { return _statusExtension; } }

        public TrackerModuleStatuses(ILoadSaveMethod loadSave) : base(loadSave)
        {
            _statusExtension = new StatusExtensionModule();
        }
    }
}
