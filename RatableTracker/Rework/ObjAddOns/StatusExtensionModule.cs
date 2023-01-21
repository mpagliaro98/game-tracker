using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
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

        protected IList<Status> Statuses { get; private set; } = new List<Status>();

        protected readonly ILoadSaveHandler<ILoadSaveMethodStatusExtension> _loadSave;
        public ILogger Logger { get; private set; }

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave, ILogger logger = null)
        {
            _loadSave = loadSave;
            Logger = logger;
        }

        public virtual void LoadData()
        {
            using (var conn = _loadSave.NewConnection())
            {
                Statuses = conn.LoadStatuses(this);
            }
        }

        public IList<Status> GetStatusList()
        {
            return new List<Status>(Statuses);
        }

        public int TotalNumStatuses()
        {
            return Statuses.Count;
        }

        internal void SaveStatus(Status status)
        {
            Logger?.Log("SaveStatus - " + status.UniqueID.ToString());
            status.Validate();

            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
            {
                if (Statuses.Count() >= LimitStatuses)
                {
                    try
                    {
                        throw new ExceededLimitException("Attempted to exceed limit of " + LimitStatuses.ToString() + " for list of statuses");
                    }
                    catch (ExceededLimitException e)
                    {
                        Logger?.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
                Statuses.Add(status);
            }

            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneStatus(status);
            }
            status.PostSave();
        }

        internal void DeleteStatus(Status status, TrackerModule module)
        {
            Logger?.Log("DeleteStatus - " + status.UniqueID.ToString());
            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
            {
                try
                {
                    throw new InvalidObjectStateException("Status " + status.Name.ToString() + " has not been saved yet and cannot be deleted");
                }
                catch (InvalidObjectStateException e)
                {
                    Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            module.RemoveReferencesToObject(status, typeof(Status));
            Statuses.Remove(status);
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneStatus(status);
            }
            status.PostDelete();
        }

        public virtual void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Status status in Statuses)
                {
                    if (status.RemoveReferenceToObject(obj, type))
                    {
                        conn.SaveOneStatus(status);
                    }
                }
            }
        }

        public virtual void ApplySettingsChanges(Settings settings)
        {
            foreach (Status status in Statuses)
            {
                status.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Status status in Statuses)
                {
                    status.Validate();
                    conn.SaveOneStatus(status);
                }
            }
        }
    }
}
