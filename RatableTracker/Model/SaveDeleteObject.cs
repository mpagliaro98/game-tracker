using RatableTracker.Exceptions;
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
    public abstract class SaveDeleteObject : SavableObject
    {
        public void Delete(TrackerModule module, Settings settings)
        {
            Delete(module, settings, null);
        }

        public void Delete(TrackerModule module, Settings settings, ILoadSaveMethod conn)
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
            catch (Exception ex)
            {
                // something went wrong when saving additional objects from event handlers or in pre/post-delete
                conn?.SetCancel(true);
                module.Logger.Log(Util.Util.FormatUnhandledExceptionMessage(ex, "Delete " + GetType().Name));
                // reload all data in the module to wipe out potentially corrupt data due to partially-changed objects
                module.LoadData(settings);
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
