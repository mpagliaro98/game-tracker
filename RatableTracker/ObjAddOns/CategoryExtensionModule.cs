using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class CategoryExtensionModule
    {
        public virtual int LimitRatingCategories => 10;

        protected IList<RatingCategory> RatingCategories { get; private set; } = new List<RatingCategory>();

        protected readonly ILoadSaveHandler<ILoadSaveMethodCategoryExtension> _loadSave;
        public Logger Logger { get; private set; }
        public IModuleCategorical BaseModule { get; internal set; }

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave) : this(loadSave, new Logger()) { }

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave, Logger logger)
        {
            _loadSave = loadSave;
            Logger = logger;
        }

        public virtual void LoadData(SettingsScore settings)
        {
            using (var conn = _loadSave.NewConnection())
            {
                RatingCategories = conn.LoadCategories(this, settings);
            }
        }

        public IList<RatingCategory> GetRatingCategoryList()
        {
            return new List<RatingCategory>(RatingCategories);
        }

        public int TotalNumRatingCategories()
        {
            return RatingCategories.Count;
        }

        internal bool SaveRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            Logger.Log("SaveRatingCategory - " + ratingCategory.UniqueID.ToString());

            bool isNew = false;
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                if (RatingCategories.Count() >= LimitRatingCategories)
                {
                    string message = "Attempted to exceed limit of " + LimitRatingCategories.ToString() + " for list of categories";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                RatingCategories.Add(ratingCategory);
                isNew = true;
            }
            else
            {
                RatingCategories.Replace(ratingCategory);
            }

            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.SaveOneCategory(ratingCategory);
                }
            }
            else
            {
                conn.SaveOneCategory(ratingCategory);
            }
            return isNew;
        }

        internal void DeleteRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            Logger.Log("DeleteRatingCategory - " + ratingCategory.UniqueID.ToString());
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                string message = "Category " + ratingCategory.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            RatingCategories.Remove(ratingCategory);
            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.DeleteOneCategory(ratingCategory);
                }
            }
            else
            {
                conn.DeleteOneCategory(ratingCategory);
            }
        }

        public virtual void RemoveReferencesToObject(IKeyable obj, Type type, TrackerModule module)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RatingCategory ratingCategory in RatingCategories)
                {
                    if (ratingCategory.RemoveReferenceToObject(obj, type))
                    {
                        ratingCategory.Save(module, conn);
                    }
                }
            }
        }

        public virtual void ApplySettingsChanges(Settings settings, TrackerModule module)
        {
            foreach (RatingCategory category in RatingCategories)
            {
                category.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RatingCategory category in RatingCategories)
                {
                    category.Save(module, conn);
                }
            }
        }

        public double SumOfCategoryWeights()
        {
            return RatingCategories.Select(cat => cat.Weight).Sum();
        }

        internal void AddCategoryValueToAllModelObjects(TrackerModule module, SettingsScore settings, RatingCategory category)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RankedObject obj in module.GetModelObjectList())
                {
                    if (obj is IModelObjectCategorical objCat)
                    {
                        objCat.CategoryExtension.CategoryValuesManual.Add(new CategoryValue(this, settings, category));
                        conn.SaveOneModelObject(obj);
                    }
                }
            }
        }

        public double GetTotalScoreFromCategoryValues(IList<CategoryValue> categoryValues)
        {
            double sumOfWeights = SumOfCategoryWeights();
            double total = categoryValues.Select(cv => (cv.RatingCategory.Weight / sumOfWeights) * cv.PointValue).Sum();
            return total;
        }
    }
}
