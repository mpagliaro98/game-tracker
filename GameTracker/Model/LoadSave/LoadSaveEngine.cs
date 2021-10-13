using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public abstract class LoadSaveEngine
    {
        public abstract IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule);
        public abstract IEnumerable<ScoreRange> LoadScoreRanges(RatingModule parentModule);
        public abstract IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule);
        public abstract Settings LoadSettings(RatingModule parentModule);
        public abstract void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects);
        public abstract void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges);
        public abstract void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories);
        public abstract void SaveSettings(Settings settings);
    }
}
