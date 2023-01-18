﻿using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.Util;
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

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            StatusExtension.LoadData();
        }

        public void TransferToNewModule(TrackerModuleScoreStatuses newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodStatusExtension connCurrent, ILoadSaveMethodStatusExtension connNew, Settings settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllStatuses(connCurrent.LoadStatuses(StatusExtension));
        }

        public override void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            base.RemoveReferencesToObject(obj, type);
            StatusExtension.RemoveReferencesToObject(obj, type);
        }
    }
}