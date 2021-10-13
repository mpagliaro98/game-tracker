using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ScoreRelationships;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework
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

        public void RecalculateScores(double minRangeOld, double maxRangeOld, double minRangeNew, double maxRangeNew)
        {
            if (minRangeOld == minRangeNew && maxRangeOld == maxRangeNew) return;
            double oldRange = maxRangeOld - minRangeOld;
            double newRange = maxRangeNew - minRangeNew;

            foreach (RatableObject ro in ratableObjects)
            {
                if (ro.IgnoreCategories)
                {
                    if (oldRange == 0)
                        ro.SetManualFinalScore(minRangeNew);
                    else
                        ro.SetManualFinalScore(((ro.FinalScore - minRangeOld) * newRange / oldRange) + minRangeNew);
                }
                else
                {
                    foreach (RatingCategoryValue rcv in ro.CategoryValues)
                    {
                        if (oldRange == 0)
                            rcv.PointValue = minRangeNew;
                        else
                            rcv.PointValue = ((rcv.PointValue - minRangeOld) * newRange / oldRange) + minRangeNew;
                    }
                }
            }
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            list = list.Append(obj);
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, T orig)
        {
            List<T> temp = list.ToList();
            int idx = temp.IndexOf(orig);
            UpdateInList(ref list, saveFunction, obj, idx);
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int idx)
        {
            List<T> temp = list.ToList();
            temp[idx] = obj;
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        public void AddRatableObject(RatableObject obj)
        {
            AddToList(ref ratableObjects, SaveRatableObjects, obj);
        }

        public void AddScoreRange(ScoreRange obj)
        {
            AddToList(ref scoreRanges, SaveScoreRanges, obj);
        }

        public void AddRatingCategory(RatingCategory obj)
        {
            AddToList(ref ratingCategories, SaveRatingCategories, obj);
        }

        public void UpdateRatableObject(RatableObject obj, RatableObject orig)
        {
            UpdateInList(ref ratableObjects, SaveRatableObjects, obj, orig);
        }

        public void UpdateScoreRange(ScoreRange obj, ScoreRange orig)
        {
            UpdateInList(ref scoreRanges, SaveScoreRanges, obj, orig);
        }

        public void UpdateRatingCategory(RatingCategory obj, RatingCategory orig)
        {
            UpdateInList(ref ratingCategories, SaveRatingCategories, obj, orig);
        }
    }
}
