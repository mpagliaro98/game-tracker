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
        public virtual IEnumerable<Platform> LoadPlatforms(RatingModule parentModule)
        {
            return LoadListParent<Platform>(parentModule);
        }

        public virtual void SavePlatforms(IEnumerable<Platform> platforms)
        {
            SaveListParent(platforms);
        }

        public override IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableGame>(parentModule);
        }

        public override IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule)
        {
            return LoadListParent<RatingCategoryWeighted>(parentModule);
        }

        public override void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects)
        {
            SaveRatableObjects(ratableObjects.Cast<RatableGame>());
        }

        public override void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories)
        {
            SaveRatingCategories(ratingCategories.Cast<RatingCategoryWeighted>());
        }
    }
}
