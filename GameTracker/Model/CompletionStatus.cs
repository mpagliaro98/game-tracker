using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class CompletionStatus : ISavable
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool useAsFinished = false;
        public bool UseAsFinished 
        {
            get { return useAsFinished;  }
            set { useAsFinished = value; }
        }

        private bool excludeFromStats = false;
        public bool ExcludeFromStats
        {
            get { return excludeFromStats; }
            set { excludeFromStats = value; }
        }

        public CompletionStatus() { }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("name", name);
            sr.SaveValue("useAsFinished", useAsFinished.ToString());
            sr.SaveValue("excludeFromStats", excludeFromStats.ToString());
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "name":
                        name = sr.GetValue(key);
                        break;
                    case "useAsFinished":
                        useAsFinished = bool.Parse(sr.GetValue(key));
                        break;
                    case "excludeFromStats":
                        excludeFromStats = bool.Parse(sr.GetValue(key));
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("CompletionStatus.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
