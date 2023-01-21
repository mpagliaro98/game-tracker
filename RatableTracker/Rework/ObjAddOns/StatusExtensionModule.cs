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

        internal void SaveStatus(Status status, TrackerModule module)
        {
            Logger?.Log("SaveStatus - " + status.UniqueID.ToString());
            status.Validate(Logger);

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
                Util.Util.SaveOne(module, status, conn, conn.SaveOneStatus);
            }
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
                Util.Util.DeleteOne(module, status, conn, conn.DeleteOneStatus);
            }
        }

        public virtual void RemoveReferencesToObject(IKeyable obj, Type type, TrackerModule module)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Status status in Statuses)
                {
                    if (status.RemoveReferenceToObject(obj, type))
                    {
                        Util.Util.SaveOne(module, status, conn, conn.SaveOneStatus);
                    }
                }
            }
        }

        public virtual void ApplySettingsChanges(Settings settings, TrackerModule module)
        {
            foreach (Status status in Statuses)
            {
                status.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Status status in Statuses)
                {
                    Util.Util.SaveOne(module, status, conn, conn.SaveOneStatus);
                }
            }
        }
    }
}
