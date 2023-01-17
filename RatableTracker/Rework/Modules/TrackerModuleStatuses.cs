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
        protected readonly StatusExtensionModule _statusExtension;
        public StatusExtensionModule StatusExtension { get { return _statusExtension; } }

        protected readonly new ILoadSaveHandler<ILoadSaveMethodStatuses> _loadSave;

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave) : this(loadSave, new StatusExtensionModule(loadSave)) { }

        public TrackerModuleStatuses(ILoadSaveHandler<ILoadSaveMethodStatuses> loadSave, StatusExtensionModule statusExtension) : base(loadSave)
        {
            _statusExtension = statusExtension;
        }

        public override void Init()
        {
            base.Init();
            StatusExtension.Init();
        }

        public void TransferToNewModule(TrackerModuleStatuses newModule)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodStatusExtension connCurrent, ILoadSaveMethodStatusExtension connNew)
        {
            base.TransferToNewModule(connCurrent, connNew);
            connNew.SaveAllStatuses(connCurrent.LoadStatuses());
        }
    }
}
