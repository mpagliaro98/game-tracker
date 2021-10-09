using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class RatableObject : ISavable, IModuleAccess
    {
        private string name = "";
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private string comment = "";
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private IEnumerable<RatingCategoryValue> categoryValues;

        private bool ignoreCategories = false;
        public bool IgnoreCategories
        {
            get { return ignoreCategories; }
            set { ignoreCategories = value; }
        }

        private double finalScoreManual = 0;
        public double FinalScore
        {
            get
            {
                if (IgnoreCategories)
                {
                    return finalScoreManual;
                }
                else
                {
                    double total = 0;
                    foreach (RatingCategoryValue categoryValue in categoryValues)
                    {
                        total += (GetParentModule().FindRatingCategory(categoryValue.RatingCategoryName).GetWeight() * categoryValue.PointValue);
                    }
                    // TODO: need to normalize total between min and max values
                    return total;
                }
            }
        }

        protected RatingModule parentModule;

        public RatableObject() { }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("name", name);
            sr.SaveValue("comment", comment);
            sr.SaveList("categoryValues", categoryValues);
            sr.SaveValue("ignoreCategories", ignoreCategories.ToString());
            sr.SaveValue("finalScoreManual", finalScoreManual.ToString());
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "name":
                        name = sr.GetValue(key);
                        break;
                    case "comment":
                        comment = sr.GetValue(key);
                        break;
                    case "categoryValues":
                        categoryValues = sr.GetListOfISavable<RatingCategoryValue>(key);
                        break;
                    case "ignoreCategories":
                        ignoreCategories = bool.Parse(sr.GetValue(key));
                        break;
                    case "finalScoreManual":
                        finalScoreManual = double.Parse(sr.GetValue(key));
                        break;
                    default:
                        Console.WriteLine("RatableObject.cs RestoreFromRepresentation: unrecognized key " + key);
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
