using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
{
    public abstract class SaveDeleteObject : SavableObject
    {
        public void Delete(TrackerModule module)
        {
            Delete(module, null);
        }

        public void Delete(TrackerModule module, ILoadSaveMethod conn)
        {
            try
            {
                PreDelete(module);
                DeleteObjectFromModule(module, conn);
                PostDelete(module);
            }
            catch
            {
                conn?.SetCancel(true);
                throw;
            }
        }

        protected virtual void PreDelete(TrackerModule module) { }

        protected abstract void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostDelete(TrackerModule module) { }

        public virtual void ApplySettingsChanges(Settings settings) { }
    }
}
