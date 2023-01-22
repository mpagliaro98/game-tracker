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

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            module.SaveSettings(this, conn);
            return false;
        }

        protected override void PostSave(TrackerModule module, bool isNew)
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
