using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
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

        protected void SetParentModule<T>(IEnumerable<T> list, RatingModule parentModule)
        {
            foreach (T obj in list)
            {
                SetParentModule(obj, parentModule);
            }
        }

        protected void SetParentModule<T>(T obj, RatingModule parentModule)
        {
            if (obj is IModuleAccess moduleAccess)
            {
                moduleAccess.SetParentModule(parentModule);
            }
        }
    }
}
