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

        public void Save(TrackerModule module)
        {
            Save(module, null);
        }

        public void Save(TrackerModule module, ILoadSaveMethod conn)
        {
            Save(module, conn, true);
        }

        protected internal void SaveWithoutValidation(TrackerModule module, ILoadSaveMethod conn)
        {
            Save(module, conn, false);
        }

        private void Save(TrackerModule module, ILoadSaveMethod conn, bool validate)
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

        protected virtual void PreSave(TrackerModule module, ILoadSaveMethod conn) { }

        protected abstract bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn) { }
    }
}
