using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class StatusExtension
    {
        private UniqueID _status = new UniqueID(false);
        public Status Status
        {
            get
            {
                if (!_status.HasValue()) return null;
                return TrackerModule.FindObjectInList(module.GetStatusList(), _status);
            }
            set
            {
                _status = value.UniqueID;
            }
        }

        private readonly StatusExtensionModule module;

        public StatusExtension(StatusExtensionModule module)
        {
            this.module = module;
        }
    }
}
