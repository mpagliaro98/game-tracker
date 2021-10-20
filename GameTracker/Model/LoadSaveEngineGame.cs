using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;

namespace GameTracker.Model
{
    public abstract class LoadSaveEngineGame<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : LoadSaveEngineRatedStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : Status, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_PLATFORMS = new LoadSaveIdentifier("Platforms");

        public virtual IEnumerable<Platform> LoadPlatforms()
        {
            return LoadISavableList<Platform>(ID_PLATFORMS);
        }

        public virtual void SavePlatforms(IEnumerable<Platform> platforms)
        {
            SaveISavableList(platforms, ID_PLATFORMS);
        }
    }
}
