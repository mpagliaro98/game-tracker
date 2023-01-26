﻿using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class TrackerModuleStatuses : TrackerModule, IModuleStatus
    {
        public StatusExtensionModule StatusExtension { get; private set; }

        protected new ILoadSaveHandler<ILoadSaveMethodStatuses> _loadSave => (ILoadSaveHandler<ILoadSaveMethodStatuses>)base._loadSave;

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave, Logger logger) : this(loadSave, new StatusExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave, StatusExtensionModule statusExtension) : this(loadSave, statusExtension, new Logger()) { }

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave, StatusExtensionModule statusExtension, Logger logger) : base(loadSave, logger)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseModule = this;
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            StatusExtension.LoadData();
        }

        public void TransferToNewModule(TrackerModuleStatuses newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodStatuses connCurrent, ILoadSaveMethodStatuses connNew, Settings settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllStatuses(connCurrent.LoadStatuses(StatusExtension));
        }

        public override void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            base.RemoveReferencesToObject(obj, type);
            StatusExtension.RemoveReferencesToObject(obj, type, this);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            StatusExtension.ApplySettingsChanges(settings, this);
        }
    }
}
