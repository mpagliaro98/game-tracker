using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class CategoryExtensionModule
    {
        public virtual int LimitRatingCategories => 10;

        protected IList<RatingCategory> RatingCategories { get; private set; } = new List<RatingCategory>();

        protected readonly ILoadSaveHandler<ILoadSaveMethodCategoryExtension> _loadSave;
        public ILogger Logger { get; private set; }

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave, ILogger logger = null)
        {
            _loadSave = loadSave;
            Logger = logger;
        }

        public virtual void LoadData()
        {
            using (var conn = _loadSave.NewConnection())
            {
                RatingCategories = conn.LoadCategories(this);
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

        internal void SaveRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            Logger?.Log("SaveRatingCategory - " + ratingCategory.UniqueID.ToString());

            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                if (RatingCategories.Count() >= LimitRatingCategories)
                {
                    string message = "Attempted to exceed limit of " + LimitRatingCategories.ToString() + " for list of categories";
                    Logger?.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                RatingCategories.Add(ratingCategory);
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
        }

        internal void DeleteRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            Logger?.Log("DeleteRatingCategory - " + ratingCategory.UniqueID.ToString());
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                string message = "Category " + ratingCategory.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger?.Log(typeof(InvalidObjectStateException).Name + ": " + message);
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
    }
}
