using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ObjectHierarchy
{
    public class RatableObjectCategorical : RatableObject, IObjectCategorical
    {
        private IEnumerable<RatingCategoryValue> categoryValues = new List<RatingCategoryValue>();
        public IEnumerable<RatingCategoryValue> CategoryValues
        {
            get { return categoryValues; }
            set { categoryValues = value; }
        }

        public bool IgnoreCategories { get; set; } = false;

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveList("categoryValues", categoryValues);
            sr.SaveValue("ignoreCategories", IgnoreCategories);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "categoryValues":
                        categoryValues = sr.GetListOfISavable<RatingCategoryValue>(key);
                        break;
                    case "ignoreCategories":
                        IgnoreCategories = sr.GetBool(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdateRatingCategoryValues(Func<RatingCategoryValue, bool> where, Action<RatingCategoryValue> action)
        {
            Util.UpdateInListOnCondition(categoryValues, where, action);
        }

        public void DeleteRatingCategoryValues(Predicate<RatingCategoryValue> where)
        {
            Util.DeleteFromListOnCondition(ref categoryValues, where);
        }
    }
}
