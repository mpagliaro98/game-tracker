using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Util
{
    public class Settings : SavableObject
    {
        public Settings() { }

        public static Settings Load(ILoadSaveHandler<ILoadSaveMethod> loadSaveHandler)
        {
            using (var conn = loadSaveHandler.NewConnection())
            {
                return conn.LoadSettings();
            }
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            module.SaveSettings(this, conn);
            return false;
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            module.ApplySettingsChanges(this, conn);
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
