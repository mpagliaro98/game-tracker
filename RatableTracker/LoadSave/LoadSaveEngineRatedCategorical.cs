using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineRatedCategorical<TListedObj, TRange, TSettings, TRatingCat>
        : LoadSaveEngineRated<TListedObj, TRange, TSettings>, ILoadSaveCategorical
        where TListedObj : RatableObjectCategorical, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        public LoadSaveIdentifier ID_RATINGCATEGORIES => new LoadSaveIdentifier("RatingCategories");
    }
}
