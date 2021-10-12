using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    public class RatingCategoryWeighted : RatingCategory
    {
        private double weight = 0;

        public RatingCategoryWeighted() { }

        public override double GetWeight()
        {
            return weight;
        }

        public void SetWeight(double val)
        {
            weight = val;
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("weight", weight.ToString());
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "weight":
                        weight = double.Parse(sr.GetValue(key));
                        break;
                    default:
                        Console.WriteLine("RatingCategoryWeighted.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
