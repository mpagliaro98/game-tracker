using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
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
        protected readonly ILogger _logger;

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave, ILogger logger = null)
        {
            _loadSave = loadSave;
            _logger = logger;
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

        internal void SaveRatingCategory(RatingCategory ratingCategory)
        {
            // TODO throw unique exception
            ratingCategory.Validate();
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                if (RatingCategories.Count() >= LimitRatingCategories)
                    throw new Exception("Attempted to exceed limit of " + LimitRatingCategories.ToString() + " for list of categories");
                RatingCategories.Add(ratingCategory);
            }
            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneCategory(ratingCategory);
            }
            ratingCategory.PostSave();
        }

        internal void DeleteRatingCategory(RatingCategory ratingCategory, TrackerModule module)
        {
            // TODO throw unique exception
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
                throw new Exception("Category " + ratingCategory.Name.ToString() + " has not been saved yet and cannot be deleted");
            module.RemoveReferencesToObject(ratingCategory, typeof(RatingCategory));
            RatingCategories.Remove(ratingCategory);
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneCategory(ratingCategory);
            }
            ratingCategory.PostDelete();
        }

        public virtual void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RatingCategory ratingCategory in RatingCategories)
                {
                    if (ratingCategory.RemoveReferenceToObject(obj, type))
                    {
                        conn.SaveOneCategory(ratingCategory);
                    }
                }
            }
        }
    }
}
