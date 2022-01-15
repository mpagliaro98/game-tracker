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
        public LoadSaveIdentifier ID_PLATFORMS => new LoadSaveIdentifier("Platforms");

        public abstract void TransferSaveFiles(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to);
        public abstract Task TransferSaveFilesAsync(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to);
    }
}
