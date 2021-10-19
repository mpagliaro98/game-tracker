using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public abstract class LoadSaveEngineGame : LoadSaveEngineCompletable
    {
        protected static LoadSaveIdentifier ID_PLATFORMS = new LoadSaveIdentifier("Platforms");

        public virtual IEnumerable<Platform> LoadPlatforms(RatingModule parentModule)
        {
            return LoadListParent<Platform>(parentModule, ID_PLATFORMS);
        }

        public virtual void SavePlatforms(IEnumerable<Platform> platforms)
        {
            SaveListParent(platforms, ID_PLATFORMS);
        }

        public override IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableGame>(parentModule, ID_RATABLEOBJECTS);
        }

        public override IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule)
        {
            return LoadListParent<RatingCategoryWeighted>(parentModule, ID_RATINGCATEGORIES);
        }

        public override IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule)
        {
            return LoadListParent<CompletionStatusGame>(parentModule, ID_COMPLETIONSTATUSES);
        }
    }
}
