using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class RatingCategoryValue : ISavable, IModuleAccess
    {
        private string ratingCategoryName = "";
        public string RatingCategoryName
        {
            get { return ratingCategoryName; }
            set { ratingCategoryName = value; }
        }

        private double pointValue = 0;
        public double PointValue
        {
            get { return pointValue; }
            set { pointValue = value; }
        }

        private RatingModule parentModule;

        public RatingCategoryValue() { }

        public RatingCategoryValue(RatingCategory ratingCategory, double pointValue)
        {
            RatingCategoryName = ratingCategory.Name;
            PointValue = pointValue;
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("ratingCategoryName", ratingCategoryName);
            sr.SaveValue("pointValue", pointValue.ToString());
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "ratingCategoryName":
                        ratingCategoryName = sr.GetValue(key);
                        break;
                    case "pointValue":
                        pointValue = double.Parse(sr.GetValue(key));
                        break;
                    default:
                        Console.WriteLine("RatingCategoryValue.cs RestoreFromRepresentation: unrecognized key " + key);
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
