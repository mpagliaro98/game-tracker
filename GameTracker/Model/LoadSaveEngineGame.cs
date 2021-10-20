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
    public abstract class LoadSaveEngineGame<TRatableObj, TRatingCat, TCompStatus>
        : LoadSaveEngineCompletable<TRatableObj, TRatingCat, TCompStatus>
        where TRatableObj : RatableGame, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
        where TCompStatus : CompletionStatusGame, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_PLATFORMS = new LoadSaveIdentifier("Platforms");

        public virtual IEnumerable<Platform> LoadPlatforms(RatingModuleCompletableGame<TRatableObj, TRatingCat, TCompStatus> parentModule)
        {
            return LoadListParent<Platform>(parentModule, ID_PLATFORMS);
        }

        public virtual void SavePlatforms(IEnumerable<Platform> platforms)
        {
            SaveListParent(platforms, ID_PLATFORMS);
        }
    }
}
