﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    abstract class LoadSaveEngine
    {
        public abstract IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule);
        public abstract IEnumerable<ScoreRange> LoadScoreRanges(RatingModule parentModule);
        public abstract IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule);
        public abstract void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects);
        public abstract void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges);
        public abstract void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories);
    }
}
