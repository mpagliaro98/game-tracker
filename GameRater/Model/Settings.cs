using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class Settings : ISavable
    {
        private int minScore = 0;
        public int MinScore
        {
            get { return minScore; }
            set { minScore = value; }
        }

        private int maxScore = 10;
        public int MaxScore
        {
            get { return maxScore; }
            set { maxScore = value; }
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
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "minScore":
                        minScore = int.Parse(sr.GetValue(key));
                        break;
                    case "maxScore":
                        maxScore = int.Parse(sr.GetValue(key));
                        break;
                    default:
                        Console.WriteLine("Settings.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
