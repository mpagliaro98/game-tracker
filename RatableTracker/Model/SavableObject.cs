using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    public abstract class SavableObject : RepresentationObject
    {
        public void Validate()
        {
            Validate(new Logger());
        }

        public void Validate(Logger logger)
        {
            PreValidate();
            try
            {
                ValidateFields();
            }
            catch (ValidationException e)
            {
                logger.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }
            PostValidate();
        }

        protected virtual void PreValidate() { }

        protected virtual void ValidateFields() { }

        protected virtual void PostValidate() { }

        public void Save(TrackerModule module, Settings settings)
        {
            Save(module, settings, null);
        }

        public void Save(TrackerModule module, Settings settings, ILoadSaveMethod conn)
        {
            Save(module, settings, conn, true);
        }

        protected internal void SaveWithoutValidation(TrackerModule module, Settings settings, ILoadSaveMethod conn)
        {
            Save(module, settings, conn, false);
        }

        private void Save(TrackerModule module, Settings settings, ILoadSaveMethod conn, bool validate)
        {
            bool newConn = false;
            try
            {
                if (conn == null)
                {
                    conn = module.GetNewConnection();
                    newConn = true;
                }
                if (validate) Validate(module.Logger);
                PreSave(module, conn);
                bool isNew = SaveObjectToModule(module, conn);
                PostSave(module, isNew, conn);
            }
            catch (ValidationException)
            {
                // data in the main object being saved is not valid
                conn?.SetCancel(true);
                throw;
            }
            catch (Exception ex)
            {
                // something went wrong when saving additional objects from event handlers or in pre/post-save
                conn?.SetCancel(true);
                module.Logger.Log(Util.Util.FormatUnhandledExceptionMessage(ex, "Save " + GetType().Name));
                // reload all data in the module to wipe out potentially corrupt data due to partially-changed objects
                module.LoadData(settings);
                throw;
            }
            finally
            {
                if (newConn) conn?.Dispose();
            }
        }

        protected virtual void PreSave(TrackerModule module, ILoadSaveMethod conn) { }

        protected abstract bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn) { }
    }
}
