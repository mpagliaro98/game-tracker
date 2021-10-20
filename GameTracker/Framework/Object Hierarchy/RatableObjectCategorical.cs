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
    public class RatableObjectCategorical : RatableObject, ICategorical
    {
        private IEnumerable<RatingCategoryValue> categoryValues = new List<RatingCategoryValue>();
        public IEnumerable<RatingCategoryValue> CategoryValues
        {
            get { return categoryValues; }
        }

        private bool ignoreCategories = false;
        public bool IgnoreCategories
        {
            get { return ignoreCategories; }
            set { ignoreCategories = value; }
        }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveList("categoryValues", categoryValues);
            sr.SaveValue("ignoreCategories", ignoreCategories);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            if (sr == null) return;
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "categoryValues":
                        categoryValues = sr.GetListOfISavable<RatingCategoryValue>(key);
                        break;
                    case "ignoreCategories":
                        ignoreCategories = sr.GetBool(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(GetType().Name + " RestoreFromRepresentation: unrecognized key " + key);
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
