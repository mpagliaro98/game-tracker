﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        public CompletionStatus() { }

        public CompletionStatus(string name, bool useAsFinished, bool excludeFromStats, Color color)
        {
            this.name = name;
            this.useAsFinished = useAsFinished;
            this.excludeFromStats = excludeFromStats;
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("useAsFinished", useAsFinished);
            sr.SaveValue("excludeFromStats", excludeFromStats);
            sr.SaveValue("color", color);
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
                    case "color":
                        color = sr.GetColor(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("CompletionStatus.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public override int GetHashCode()
        {
            return ReferenceKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CompletionStatus o = (CompletionStatus)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
