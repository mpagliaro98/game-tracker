﻿using RatableTracker.Rework.Exceptions;
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

        internal void SaveRatingCategory(RatingCategory ratingCategory, TrackerModule module)
        {
            Logger?.Log("SaveRatingCategory - " + ratingCategory.UniqueID.ToString());
            ratingCategory.Validate(Logger);

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
                        Logger?.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
                RatingCategories.Add(ratingCategory);
            }

            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneCategory(ratingCategory);
            }
            ratingCategory.PostSave(module);
        }

        internal void DeleteRatingCategory(RatingCategory ratingCategory, TrackerModule module)
        {
            Logger?.Log("DeleteRatingCategory - " + ratingCategory.UniqueID.ToString());
            if (Util.Util.FindObjectInList(RatingCategories, ratingCategory.UniqueID) == null)
            {
                try
                {
                    throw new InvalidObjectStateException("Category " + ratingCategory.Name.ToString() + " has not been saved yet and cannot be deleted");
                }
                catch (InvalidObjectStateException e)
                {
                    Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            module.RemoveReferencesToObject(ratingCategory, typeof(RatingCategory));
            RatingCategories.Remove(ratingCategory);
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneCategory(ratingCategory);
            }
            ratingCategory.PostDelete(module);
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
                    category.Validate(Logger);
                    conn.SaveOneCategory(category);
                    category.PostSave(module);
                }
            }
        }
    }
}
