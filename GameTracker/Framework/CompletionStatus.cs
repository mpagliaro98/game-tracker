using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class CompletionStatus : ISavable, IReferable
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

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        public CompletionStatus()
        {
            referenceKey = Guid.NewGuid();
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("useAsFinished", useAsFinished);
            sr.SaveValue("excludeFromStats", excludeFromStats);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "referenceKey":
                        referenceKey = sr.GetGuid(key);
                        break;
                    case "name":
                        name = sr.GetString(key);
                        break;
                    case "useAsFinished":
                        useAsFinished = sr.GetBool(key);
                        break;
                    case "excludeFromStats":
                        excludeFromStats = sr.GetBool(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("CompletionStatus.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
