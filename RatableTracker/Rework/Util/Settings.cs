using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Rework.Util
{
    public class Settings : SavableObject
    {
        public Settings() { }

        public void Validate(TrackerModule module)
        {
            try
            {
                ValidateFields();
            }
            catch (ValidationException e)
            {
                module.Logger?.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }
            PostValidate();
        }

        protected virtual void ValidateFields() { }

        protected virtual void PostValidate() { }

        public void Save(TrackerModule module)
        {
            module.SaveSettings(this);
        }

        public virtual void PostSave(TrackerModule module)
        {
            module.ApplySettingsChanges(this);
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            return base.LoadIntoRepresentation();
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    default:
                        break;
                }
            }
        }
    }
}
