using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class RatingCategoryValue : ISavable
    {
        private ObjectReference ratingCategory = new ObjectReference();
        public ObjectReference RefRatingCategory => ratingCategory;

        public double PointValue { get; set; } = 0;

        public RatingCategoryValue() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("ratingCategory", ratingCategory);
            sr.SaveValue("pointValue", PointValue);
            return sr;
        }

        public virtual void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "ratingCategory":
                        ratingCategory = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "pointValue":
                        PointValue = sr.GetDouble(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetRatingCategory<T>(T obj) where T : RatingCategory, IReferable
        {
            ratingCategory.SetReference(obj);
        }
    }
}
