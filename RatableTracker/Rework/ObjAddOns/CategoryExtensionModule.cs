using RatableTracker.Framework;
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
            _logger?.Log("SaveRatingCategory - " + ratingCategory.UniqueID.ToString());
            try
            {
                ratingCategory.Validate();
            }
            catch (ValidationException e)
            {
                _logger?.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }

            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                if (RatingCategories.Count() >= LimitRatingCategories)
                {
                    try
                    {
                        throw new ExceededLimitException("Attempted to exceed limit of " + LimitRatingCategories.ToString() + " for list of categories");
                    }
                    catch (ExceededLimitException e)
                    {
                        _logger?.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
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
            _logger?.Log("DeleteRatingCategory - " + ratingCategory.UniqueID.ToString());
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                try
                {
                    throw new InvalidObjectStateException("Category " + ratingCategory.Name.ToString() + " has not been saved yet and cannot be deleted");
                }
                catch (InvalidObjectStateException e)
                {
                    _logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
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
