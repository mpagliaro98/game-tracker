using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class TrackerModuleScoreStatuses : TrackerModuleScores, IModuleStatus
    {
        public StatusExtensionModule StatusExtension { get; private set; }

        protected new ILoadSaveHandler<ILoadSaveMethodScoreStatuses> _loadSave => (ILoadSaveHandler<ILoadSaveMethodScoreStatuses>)base._loadSave;

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, Logger logger) : this(loadSave, new StatusExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, StatusExtensionModule statusExtension) : this(loadSave, statusExtension, new Logger()) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, StatusExtensionModule statusExtension, Logger logger) : base(loadSave, logger)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseModule = this;
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            StatusExtension.LoadData();
        }

        public void TransferToNewModule(TrackerModuleScoreStatuses newModule, SettingsScore settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodScoreStatuses connCurrent, ILoadSaveMethodScoreStatuses connNew, SettingsScore settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllStatuses(connCurrent.LoadStatuses(StatusExtension));
        }

        public override void ApplySettingsChanges(Settings settings, ILoadSaveMethod conn)
        {
            base.ApplySettingsChanges(settings, conn);
            StatusExtension.ApplySettingsChanges(settings, this, conn as ILoadSaveMethodStatusExtension);
        }
    }
}
