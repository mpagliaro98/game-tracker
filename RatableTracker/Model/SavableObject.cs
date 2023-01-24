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
            try
            {
                Validate(module.Logger);
                PreSave(module);
                bool isNew = SaveObjectToModule(module, conn);
                PostSave(module, isNew);
            }
            catch
            {
                conn?.SetCancel(true);
                throw;
            }
        }

        protected virtual void PreSave(TrackerModule module) { }

        protected abstract bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn);

        protected virtual void PostSave(TrackerModule module, bool isNew) { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }
    }
}
