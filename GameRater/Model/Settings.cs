using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
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
            }
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("minScore", minScore.ToString());
            sr.SaveValue("maxScore", maxScore.ToString());
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
                        minScore = double.Parse(sr.GetValue(key));
                        break;
                    case "maxScore":
                        maxScore = double.Parse(sr.GetValue(key));
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
