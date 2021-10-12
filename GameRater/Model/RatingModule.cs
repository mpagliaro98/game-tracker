using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    public abstract class RatingModule
    {
        protected IEnumerable<RatableObject> ratableObjects;
        protected IEnumerable<ScoreRange> scoreRanges;
        protected IEnumerable<RatingCategory> ratingCategories;
        protected IEnumerable<ScoreRelationship> scoreRelationships;
        protected Settings settings;

        public IEnumerable<RatableObject> RatableObjects
        {
            get { return ratableObjects; }
        }

        public IEnumerable<ScoreRange> ScoreRanges
        {
            get { return scoreRanges; }
        }

        public IEnumerable<RatingCategory> RatingCategories
        {
            get { return ratingCategories; }
        }

        public IEnumerable<ScoreRelationship> ScoreRelationships
        {
            get { return scoreRelationships; }
        }

        public Settings Settings 
        {
            get { return settings; }
        }

        public RatingModule()
        {
            scoreRelationships = new List<ScoreRelationship>();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipBetween()).ToList();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipAbove()).ToList();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipBelow()).ToList();
        }

        public virtual void Init()
        {
            LoadSettings();
            LoadScoreRanges();
            LoadRatingCategories();
            LoadRatableObjects();
        }

        protected abstract void LoadRatableObjects();
        protected abstract void LoadScoreRanges();
        protected abstract void LoadRatingCategories();
        protected abstract void LoadSettings();
        public abstract void SaveRatableObjects();
        public abstract void SaveScoreRanges();
        public abstract void SaveRatingCategories();
        public abstract void SaveSettings();

        public RatableObject FindRatableObject(string name)
        {
            foreach (RatableObject ro in ratableObjects)
            {
                if (ro.Name == name)
                {
                    return ro;
                }
            }
            throw new NameNotFoundException("RatingModule FindRatableObject: could not find name of " + name);
        }

        public ScoreRange FindScoreRange(string name)
        {
            foreach (ScoreRange sr in scoreRanges)
            {
                if (sr.Name == name)
                {
                    return sr;
                }
            }
            throw new NameNotFoundException("RatingModule FindScoreRange: could not find name of " + name);
        }

        public RatingCategory FindRatingCategory(string name)
        {
            foreach (RatingCategory rc in ratingCategories)
            {
                if (rc.Name == name)
                {
                    return rc;
                }
            }
            throw new NameNotFoundException("RatingModule FindRatingCategory: could not find name of " + name);
        }

        public ScoreRelationship FindScoreRelationship(string name)
        {
            foreach (ScoreRelationship sr in scoreRelationships)
            {
                if (sr.Name == name)
                {
                    return sr;
                }
            }
            throw new NameNotFoundException("RatingModule FindScoreRelationship: could not find name of " + name);
        }

        public Settings GetSettings()
        {
            return settings;
        }
    }
}
