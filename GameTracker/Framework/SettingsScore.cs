using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class SettingsScore : Settings
    {
        public double MinScore { get; set; } = 0;

        public double MaxScore { get; set; } = 10;

        public SettingsScore() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("minScore", MinScore);
            sr.SaveValue("maxScore", MaxScore);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "minScore":
                        MinScore = sr.GetDouble(key);
                        break;
                    case "maxScore":
                        MaxScore = sr.GetDouble(key);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
