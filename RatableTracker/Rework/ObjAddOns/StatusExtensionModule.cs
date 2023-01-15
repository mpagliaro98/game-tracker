using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class StatusExtensionModule
    {
        public virtual int LimitStatuses => 20;

        protected IList<Status> Statuses => new List<Status>();

        protected readonly TrackerModule module;

        public StatusExtensionModule(TrackerModule module)
        {
            this.module = module;
        }

        public IList<Status> GetStatusList()
        {
            return Statuses;
        }

        public void AddStatus(Status status)
        {
            // TODO validate, add, save (limit)
        }

        public void DeleteStatus(Status status)
        {
            // TODO delete, save
        }
    }
}
