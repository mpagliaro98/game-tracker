﻿using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class StatusExtensionModule
    {
        public virtual int LimitStatuses => 20;

        protected IList<Status> Statuses { get; private set; } = new List<Status>();

        public delegate void StatusDeleteHandler(object sender, StatusDeleteArgs args);
        public event StatusDeleteHandler StatusDeleted;

        protected readonly ILoadSaveHandler<ILoadSaveMethodStatusExtension> _loadSave;
        public Logger Logger { get; private set; }
        public IModuleStatus BaseModule { get; internal set; }

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave) : this(loadSave, new Logger()) { }

        public StatusExtensionModule(ILoadSaveHandler<ILoadSaveMethodStatusExtension> loadSave, Logger logger)
        {
            _loadSave = loadSave;
            Logger = logger;
        }

        public virtual void LoadData(Settings settings)
        {
            using (var conn = _loadSave.NewConnection())
            {
                Statuses = conn.LoadStatuses(BaseModule, settings);
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

        internal bool SaveStatus(Status status, TrackerModule module, ILoadSaveMethodStatusExtension conn)
        {
            Logger.Log("SaveStatus - " + status.UniqueID.ToString());
            bool isNew = false;
            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
            {
                if (Statuses.Count() >= LimitStatuses)
                {
                    string message = "Attempted to exceed limit of " + LimitStatuses.ToString() + " for list of statuses";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                Statuses.Add(status);
                isNew = true;
            }
            else
            {
                var old = Statuses.Replace(status);
                if (old != status)
                    old.Dispose();
            }
            conn.SaveOneStatus(status);
            return isNew;
        }

        internal void DeleteStatus(Status status, TrackerModule module, ILoadSaveMethodStatusExtension conn)
        {
            Logger.Log("DeleteStatus - " + status.UniqueID.ToString());
            if (Util.Util.FindObjectInList(Statuses, status.UniqueID) == null)
            {
                string message = "Status " + status.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            Statuses.Remove(status);
            conn.DeleteOneStatus(status);
            StatusDeleted?.Invoke(this, new StatusDeleteArgs(status, status.GetType(), conn));
        }

        public virtual void ApplySettingsChanges(Settings settings, TrackerModule module, ILoadSaveMethodStatusExtension conn)
        {
            foreach (Status status in Statuses)
            {
                status.ApplySettingsChanges(settings);
                status.Save(module, conn);
            }
        }
    }
}
