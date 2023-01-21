using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
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
            try
            {
                Validate(module.Logger);
                PreSave(module);
                SaveObjectToModule(module, conn);
                PostSave(module);
            }
            catch
            {
                conn?.SetCancel(true);
                throw;
            }
        }

        protected virtual void PreSave(TrackerModule module) { }

        protected abstract void SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostSave(TrackerModule module) { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }
    }
}
