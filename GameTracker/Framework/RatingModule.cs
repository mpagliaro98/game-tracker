using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ScoreRelationships;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework
{
    public abstract class RatingModule<TRatableObj, TRatingCat>
        where TRatableObj : RatableObject
        where TRatingCat : RatingCategory
    {
        protected IEnumerable<TRatableObj> ratableObjects;
        protected IEnumerable<ScoreRange> scoreRanges;
        protected IEnumerable<TRatingCat> ratingCategories;
        protected IEnumerable<ScoreRelationship> scoreRelationships;
        protected Settings settings;

        public IEnumerable<TRatableObj> RatableObjects
        {
            get { return ratableObjects; }
        }

        public IEnumerable<ScoreRange> ScoreRanges
        {
            get { return scoreRanges; }
        }

        public IEnumerable<TRatingCat> RatingCategories
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

        public virtual int LimitScoreRanges => 20;
        public virtual int LimitRatingCategories => 10;

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

        protected T FindObject<T>(IEnumerable<T> sourceList, ObjectReference objectKey) where T : IReferable
        {
            if (!objectKey.HasReference()) return default;
            foreach (IReferable obj in sourceList)
            {
                if (objectKey.IsReferencedObject(obj))
                {
                    return (T)obj;
                }
            }
            throw new ReferenceNotFoundException("RatingModule: could not find object in " + sourceList.ToString() + " with key " + objectKey.ObjectKey.ToString());
        }

        public TRatableObj FindRatableObject(ObjectReference objectKey)
        {
            return FindObject(ratableObjects, objectKey);
        }

        public ScoreRange FindScoreRange(ObjectReference objectKey)
        {
            return FindObject(scoreRanges, objectKey);
        }

        public TRatingCat FindRatingCategory(ObjectReference objectKey)
        {
            return FindObject(ratingCategories, objectKey);
        }

        public ScoreRelationship FindScoreRelationship(ObjectReference objectKey)
        {
            return FindObject(scoreRelationships, objectKey);
        }

        public void RecalculateScores(double minRangeOld, double maxRangeOld, double minRangeNew, double maxRangeNew)
        {
            if (minRangeOld == minRangeNew && maxRangeOld == maxRangeNew) return;
            double oldRange = maxRangeOld - minRangeOld;
            double newRange = maxRangeNew - minRangeNew;

            foreach (TRatableObj ro in ratableObjects)
            {
                if (ro.IgnoreCategories)
                {
                    if (oldRange == 0)
                        ro.FinalScoreManual = minRangeNew;
                    else
                        ro.FinalScoreManual = ((CalculateFinalScore(ro) - minRangeOld) * newRange / oldRange) + minRangeNew;
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

        public ScoreRange ApplyScoreRange(double score)
        {
            foreach (ScoreRange sr in ScoreRanges)
            {
                ScoreRelationship relationship = FindScoreRelationship(sr.RefScoreRelationship);
                if (relationship.IsValueInRange(score, sr.ValueList))
                {
                    return sr;
                }
            }
            return null;
        }

        public double SumOfWeights(TRatableObj ro)
        {
            double sum = 0;
            foreach (RatingCategoryValue rcv in ro.CategoryValues)
            {
                TRatingCat cat = FindRatingCategory(rcv.RefRatingCategory);
                sum += cat.Weight;
            }
            return sum;
        }

        public double CalculateFinalScore(TRatableObj ro)
        {
            if (ro.IgnoreCategories)
            {
                return ro.FinalScoreManual;
            }
            else
            {
                double total = 0;
                double sumOfWeights = SumOfWeights(ro);
                foreach (RatingCategoryValue categoryValue in ro.CategoryValues)
                {
                    TRatingCat cat = FindRatingCategory(categoryValue.RefRatingCategory);
                    double categoryWeight = cat.Weight;
                    total += (categoryWeight / sumOfWeights) * categoryValue.PointValue;
                }
                return total;
            }
        }

        public void SetMinScoreAndUpdate(double newVal)
        {
            double oldMinScore = Settings.MinScore;
            Settings.MinScore = newVal;
            RecalculateScores(oldMinScore, Settings.MaxScore, Settings.MinScore, Settings.MaxScore);
            if (GlobalSettings.Autosave)
            {
                SaveSettings();
                SaveRatableObjects();
            }
        }

        public void SetMaxScoreAndUpdate(double newVal)
        {
            double oldMaxScore = Settings.MaxScore;
            Settings.MaxScore = newVal;
            RecalculateScores(Settings.MinScore, oldMaxScore, Settings.MinScore, Settings.MaxScore);
            if (GlobalSettings.Autosave)
            {
                SaveSettings();
                SaveRatableObjects();
            }
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            AddToList(ref list, saveFunction, obj, -1);
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int limit)
        {
            if (limit >= 0 && list.Count() >= limit)
            {
                throw new ExceededLimitException("Attempted to exceed limit of " + limit.ToString() + " for list of " + typeof(T).ToString());
            }
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

        protected void DeleteFromList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            List<T> temp = list.ToList();
            temp.Remove(obj);
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        public void AddRatableObject(TRatableObj obj)
        {
            AddToList(ref ratableObjects, SaveRatableObjects, obj);
        }

        public void AddScoreRange(ScoreRange obj)
        {
            AddToList(ref scoreRanges, SaveScoreRanges, obj, LimitScoreRanges);
        }

        public void AddRatingCategory(TRatingCat obj)
        {
            AddToList(ref ratingCategories, SaveRatingCategories, obj, LimitRatingCategories);
        }

        public void UpdateRatableObject(TRatableObj obj, TRatableObj orig)
        {
            UpdateInList(ref ratableObjects, SaveRatableObjects, obj, orig);
        }

        public void UpdateScoreRange(ScoreRange obj, ScoreRange orig)
        {
            UpdateInList(ref scoreRanges, SaveScoreRanges, obj, orig);
        }

        public void UpdateRatingCategory(TRatingCat obj, TRatingCat orig)
        {
            UpdateInList(ref ratingCategories, SaveRatingCategories, obj, orig);
        }

        public void DeleteRatableObject(TRatableObj obj)
        {
            DeleteFromList(ref ratableObjects, SaveRatableObjects, obj);
        }

        public void DeleteScoreRange(ScoreRange obj)
        {
            DeleteFromList(ref scoreRanges, SaveScoreRanges, obj);
        }

        public void DeleteRatingCategory(TRatingCat obj)
        {
            DeleteFromList(ref ratingCategories, SaveRatingCategories, obj);
            ratableObjects.ForEach(ro => ro.DeleteRatingCategoryValues(rcv => rcv.RefRatingCategory.IsReferencedObject(obj)));
            if (GlobalSettings.Autosave) SaveRatableObjects();
        }
    }
}
