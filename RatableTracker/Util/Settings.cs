using RatableTracker.Events;
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
        public delegate void SettingsChangeHandler(object sender, SettingsChangeArgs args);
        public event SettingsChangeHandler SettingsChanged;

        public Settings() { }

        public static Settings Load(ILoadSaveHandler<ILoadSaveMethod> loadSaveHandler)
        {
            using var conn = loadSaveHandler.NewConnection();
            return conn.LoadSettings();
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            module.Logger.Log("Save " + GetType().Name);
            conn.SaveSettings(this);
            return false;
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            SettingsChanged?.Invoke(this, new SettingsChangeArgs(GetType(), conn));
        }
    }
}
