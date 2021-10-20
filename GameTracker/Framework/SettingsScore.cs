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
        private double minScore = 0;
        public double MinScore
        {
            get { return minScore; }
            set { minScore = value; }
        }

        private double maxScore = 10;
        public double MaxScore
        {
            get { return maxScore; }
            set { maxScore = value; }
        }

        public SettingsScore() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("minScore", minScore);
            sr.SaveValue("maxScore", maxScore);
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
                        minScore = sr.GetDouble(key);
                        break;
                    case "maxScore":
                        maxScore = sr.GetDouble(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(GetType().Name + " RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
