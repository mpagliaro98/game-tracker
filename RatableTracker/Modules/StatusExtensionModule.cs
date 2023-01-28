using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class StatusExtensionModule : ModuleExtensionBase
    {
        public virtual int LimitStatuses => 20;

        private IList<Status> _statuses = new List<Status>();
        protected IList<Status> Statuses { get { return _statuses; } private set { _statuses = value; } }

        public delegate void StatusDeleteHandler(object sender, StatusDeleteArgs args);
        public event StatusDeleteHandler StatusDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodStatusExtension> LoadSave => (ILoadSaveHandler<ILoadSaveMethodStatusExtension>)base.LoadSave;
        public new IModuleStatus BaseModule { get { return (IModuleStatus)base.BaseModule; } internal set { base.BaseModule = (ModuleBase)value; } }

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave) : base(loadSave) { }

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave, Logger logger) : base(loadSave, logger) { }

        public virtual void LoadData(Settings settings)
        {
            LoadTrackerObjectList(ref _statuses, (conn) => ((ILoadSaveMethodStatusExtension)conn).LoadStatuses(BaseModule, settings));
        }

        public IList<Status> GetStatusList()
        {
            return GetTrackerObjectList(Statuses, null, null);
        }

        public int TotalNumStatuses()
        {
            return Statuses.Count;
        }

        internal bool SaveStatus(Status status, TrackerModule module, ILoadSaveMethodStatusExtension conn)
        {
            return SaveTrackerObject(status, ref _statuses, LimitStatuses, conn.SaveOneStatus);
        }

        internal void DeleteStatus(Status status, TrackerModule module, ILoadSaveMethodStatusExtension conn)
        {
            DeleteTrackerObject(status, ref _statuses, conn.DeleteOneStatus,
                (obj) => StatusDeleted?.Invoke(this, new StatusDeleteArgs(obj, obj.GetType(), conn)), () => StatusDeleted == null ? 0 : StatusDeleted.GetInvocationList().Length);
        }
    }
}
