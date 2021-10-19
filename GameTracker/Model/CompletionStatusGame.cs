﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public class CompletionStatusGame : CompletionStatus
    {
        private bool useAsFinished = false;
        public bool UseAsFinished
        {
            get { return useAsFinished; }
            set { useAsFinished = value; }
        }

        private bool excludeFromStats = false;
        public bool ExcludeFromStats
        {
            get { return excludeFromStats; }
            set { excludeFromStats = value; }
        }

        public CompletionStatusGame() : base() { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("useAsFinished", useAsFinished);
            sr.SaveValue("excludeFromStats", excludeFromStats);
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "useAsFinished":
                        useAsFinished = sr.GetBool(key);
                        break;
                    case "excludeFromStats":
                        excludeFromStats = sr.GetBool(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("CompletionStatusGame.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
