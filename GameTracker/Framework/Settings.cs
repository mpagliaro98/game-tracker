using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework
{
    public class Settings : ISavable, IModuleAccess
    {
        private RatingModule parentModule;

        private double minScore = 0;
        public double MinScore
        {
            get { return minScore; }
            set
            {
                double oldMinScore = minScore;
                minScore = value;
                parentModule.RecalculateScores(oldMinScore, MaxScore, minScore, MaxScore);
                if (GlobalSettings.Autosave)
                {
                    GetParentModule().SaveSettings();
                    GetParentModule().SaveRatableObjects();
                }
            }
        }

        private double maxScore = 10;
        public double MaxScore
        {
            get { return maxScore; }
            set
            {
                double oldMaxScore = maxScore;
                maxScore = value;
                parentModule.RecalculateScores(MinScore, oldMaxScore, MinScore, maxScore);
                if (GlobalSettings.Autosave)
                {
                    GetParentModule().SaveSettings();
                    GetParentModule().SaveRatableObjects();
                }
            }
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("minScore", minScore);
            sr.SaveValue("maxScore", maxScore);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
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
                        System.Diagnostics.Debug.WriteLine("Settings.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public RatingModule GetParentModule()
        {
            return parentModule;
        }

        public void SetParentModule(RatingModule parentModule)
        {
            this.parentModule = parentModule;
        }
    }
}
