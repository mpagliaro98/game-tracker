using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
{
    public abstract class SaveDeleteObject : SavableObject
    {
        public abstract void Delete(TrackerModule module);

        internal virtual void PostDelete(TrackerModule module) { }
    }
}
