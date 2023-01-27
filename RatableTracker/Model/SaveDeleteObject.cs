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
            bool newConn = false;
            try
            {
                if (conn == null)
                {
                    conn = module.GetNewConnection();
                    newConn = true;
                }
                PreDelete(module, conn);
                DeleteObjectFromModule(module, conn);
                PostDelete(module, conn);
            }
            catch
            {
                conn?.SetCancel(true);
                throw;
            }
            finally
            {
                if (newConn) conn?.Dispose();
            }
        }

        protected virtual void PreDelete(TrackerModule module, ILoadSaveMethod conn) { }

        protected abstract void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostDelete(TrackerModule module, ILoadSaveMethod conn) { }
    }
}
