using RatableTracker.Interfaces;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    public abstract class SaveDeleteObject : SavableObject, IKeyable
    {
        public abstract UniqueID UniqueID { get; protected set; }

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
                module.RemoveReferencesToObject(this, GetType());
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
