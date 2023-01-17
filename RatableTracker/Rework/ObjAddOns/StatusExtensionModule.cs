using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class StatusExtensionModule
    {
        public virtual int LimitStatuses => 20;

        private IList<Status> _statuses = new List<Status>();
        protected IList<Status> Statuses => _statuses;

        protected readonly ILoadSaveHandler<ILoadSaveMethodStatusExtension> _loadSave;

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave)
        {
            _loadSave = loadSave;
        }

        public virtual void Init()
        {
            using (var conn = _loadSave.NewConnection())
            {
                _statuses = conn.LoadStatuses();
            }
        }

        public IList<Status> GetStatusList()
        {
            return Statuses;
        }

        public int TotalNumStatuses()
        {
            return Statuses.Count;
        }

        public void SaveStatus(Status status)
        {
            // TODO throw unique exception
            status.Validate();
            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
            {
                if (Statuses.Count() >= LimitStatuses)
                    throw new Exception("Attempted to exceed limit of " + LimitStatuses.ToString() + " for list of statuses");
                Statuses.Add(status);
            }
            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneStatus(status);
            }
        }

        public void DeleteStatus(Status status)
        {
            // TODO throw unique exception
            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
                throw new Exception("Status " + status.Name.ToString() + " has not been saved yet and cannot be deleted");
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneStatus(status);
            }
        }
    }
}
