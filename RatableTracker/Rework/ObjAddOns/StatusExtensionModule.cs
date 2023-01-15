using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class StatusExtensionModule
    {
        protected IList<Status> Statuses => new List<Status>();

        public StatusExtensionModule() { }

        public IList<Status> GetStatusList()
        {
            return Statuses;
        }
    }
}
