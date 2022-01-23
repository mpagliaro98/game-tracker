using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineRatedStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : LoadSaveEngineRatedStatus<TListedObj, TRange, TSettings, TStatus>, ILoadSaveCategorical
        where TListedObj : RatableObjectStatusCategorical, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : Status, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        public LoadSaveIdentifier ID_RATINGCATEGORIES => new LoadSaveIdentifier("RatingCategories");
    }
}
