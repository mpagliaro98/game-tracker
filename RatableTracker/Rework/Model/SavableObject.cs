using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
{
    public abstract class SavableObject : RepresentationObject
    {
        public void Validate(ILogger logger = null)
        {
            try
            {
                ValidateFields();
            }
            catch (ValidationException e)
            {
                logger?.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }
            PostValidate();
        }

        protected virtual void ValidateFields() { }

        protected virtual void PostValidate() { }

        public abstract void Save(TrackerModule module);

        internal virtual void PostSave(TrackerModule module) { }

        public abstract void Delete(TrackerModule module);

        internal virtual void PostDelete(TrackerModule module) { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }
    }
}
