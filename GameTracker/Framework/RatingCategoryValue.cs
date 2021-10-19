using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class RatingCategoryValue : ISavable, IModuleAccess
    {
        private ObjectReference ratingCategory = new ObjectReference();
        public RatingCategory RatingCategory
        {
            get
            {
                return ratingCategory.HasReference() ? parentModule.FindRatingCategory(ratingCategory) : null;
            }
        }

        private double pointValue = 0;
        public double PointValue
        {
            get { return pointValue; }
            set { pointValue = value; }
        }

        private RatingModule parentModule;
        public RatingModule ParentModule
        {
            get { return parentModule; }
            set { parentModule = value; }
        }

        public RatingCategoryValue() { }

        public RatingCategoryValue(RatingModule parentModule)
        {
            this.parentModule = parentModule;
        }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("ratingCategory", ratingCategory);
            sr.SaveValue("pointValue", pointValue);
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "ratingCategory":
                        ratingCategory = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "pointValue":
                        pointValue = sr.GetDouble(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("RatingCategoryValue.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
