using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
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
        public StatusExtensionModule StatusExtension { get; init; }

        protected new ILoadSaveHandler<ILoadSaveMethodScoreStatuses> LoadSave => (ILoadSaveHandler<ILoadSaveMethodScoreStatuses>)base.LoadSave;

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, Logger logger) : this(loadSave, new StatusExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, StatusExtensionModule statusExtension) : this(loadSave, statusExtension, new Logger()) { }

        public TrackerModuleScoreStatuses(ILoadSaveHandler<ILoadSaveMethodScoreStatuses> loadSave, StatusExtensionModule statusExtension, Logger logger) : base(loadSave, logger)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseModule = this;
        }

        protected override void LoadDataConsecutively(Settings settings, ILoadSaveMethod conn)
        {
            base.LoadDataConsecutively(settings, conn);
            StatusExtension.LoadDataConsecutively(settings, conn);
        }

        protected override IList<Task> LoadDataCreateTaskList(Settings settings, ILoadSaveMethod conn)
        {
            var list = base.LoadDataCreateTaskList(settings, conn);
            return list.Concat(StatusExtension.LoadDataCreateTaskList(settings, conn)).ToList();
        }

        public void TransferToNewModule(TrackerModuleScoreStatuses newModule, SettingsScore settings)
        {
            using var connCurrent = LoadSave.NewConnection();
            using var connNew = newModule.LoadSave.NewConnection();
            TransferToNewModule(connCurrent, connNew, settings);
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodScoreStatuses connCurrent, ILoadSaveMethodScoreStatuses connNew, SettingsScore settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllStatuses(connCurrent.LoadStatuses(this, settings));
        }
    }
}
