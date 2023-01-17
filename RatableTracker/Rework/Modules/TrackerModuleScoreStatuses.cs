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

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScoreStatuses> _loadSave;

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave) : this(loadSave, new StatusExtensionModule(loadSave)) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, StatusExtensionModule statusExtension) : base(loadSave)
        {
            _statusExtension = statusExtension;
        }

        public override void Init()
        {
            base.Init();
            StatusExtension.Init();
        }
    }
}
