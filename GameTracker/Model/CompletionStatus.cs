using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public class CompletionStatus : Status
    {
        public bool UseAsFinished { get; set; } = false;

        public bool ExcludeFromStats { get; set; } = false;

        public CompletionStatus() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("useAsFinished", UseAsFinished);
            sr.SaveValue("excludeFromStats", ExcludeFromStats);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "useAsFinished":
                        UseAsFinished = sr.GetBool(key);
                        break;
                    case "excludeFromStats":
                        ExcludeFromStats = sr.GetBool(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(GetType().Name + " RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
