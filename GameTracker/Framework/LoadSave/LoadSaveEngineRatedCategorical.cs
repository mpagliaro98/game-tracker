﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineRatedCategorical<TListedObj, TRange, TSettings, TRatingCat>
        : LoadSaveEngineRated<TListedObj, TRange, TSettings>, ILoadSaveCategorical<TRatingCat>
        where TListedObj : RatableObjectCategorical, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_RATINGCATEGORIES = new LoadSaveIdentifier("RatingCategories");

        public virtual IEnumerable<TRatingCat> LoadRatingCategories()
        {
            return LoadISavableList<TRatingCat>(ID_RATINGCATEGORIES);
        }

        public virtual void SaveRatingCategories(IEnumerable<TRatingCat> ratingCategories)
        {
            SaveISavableList(ratingCategories, ID_RATINGCATEGORIES);
        }
    }
}
