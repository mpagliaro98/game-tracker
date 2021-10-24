using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ScoreRelationships;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.ObjectHierarchy;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RatingModule<TListedObj, TRange, TSettings>
        : RankingModule<TListedObj, TRange, TSettings>
        where TListedObj : RatableObject
        where TRange : ScoreRange
        where TSettings : SettingsScore
    {
        public void RecalculateScores(double minRangeOld, double maxRangeOld, double minRangeNew, double maxRangeNew)
        {
            if (minRangeOld == minRangeNew && maxRangeOld == maxRangeNew) return;
            double oldRange = maxRangeOld - minRangeOld;
            double newRange = maxRangeNew - minRangeNew;

            foreach (TListedObj obj in listedObjs)
            {
                ScaleScoreOfObject(obj, oldRange, newRange, minRangeOld, minRangeNew);
            }
        }

        protected virtual void ScaleScoreOfObject(TListedObj obj, double oldRange, double newRange, double minRangeOld, double minRangeNew)
        {
            if (oldRange == 0)
                obj.FinalScoreManual = minRangeNew;
            else
                obj.FinalScoreManual = ((GetScoreOfObject(obj) - minRangeOld) * newRange / oldRange) + minRangeNew;
        }

        public override System.Drawing.Color GetRangeColorFromObject(TListedObj obj)
        {
            double score = GetScoreOfObject(obj);
            return GetRangeColorFromValue(score);
        }

        public virtual double GetScoreOfObject(TListedObj obj)
        {
            return obj.FinalScoreManual;
        }

        public virtual void SetScoresAndUpdate(double newMin, double newMax)
        {
            double oldMinScore = Settings.MinScore;
            double oldMaxScore = Settings.MaxScore;
            Settings.MinScore = newMin;
            Settings.MaxScore = newMax;
            RecalculateScores(oldMinScore, oldMaxScore, Settings.MinScore, Settings.MaxScore);
            if (GlobalSettings.Autosave)
            {
                SaveSettings();
                SaveListedObjects();
            }
        }

        public virtual void ValidateScore(double val)
        {
            if (val < Settings.MinScore || val > Settings.MaxScore)
                throw new ValidationException("Score must be between " + Settings.MinScore.ToString() + " and " + Settings.MaxScore.ToString());
        }

        public override int GetRankOfObject(TListedObj obj)
        {
            return GetRankOfScore(GetScoreOfObject(obj));
        }

        public virtual int GetRankOfScore(double score)
        {
            return GetRankOfScore(score, ListedObjects, null, null);
        }

        public virtual int GetRankOfScore(double score, TListedObj obj)
        {
            return GetRankOfScore(score, ListedObjects, null, obj);
        }

        protected int GetRankOfScore(double score, IEnumerable<TListedObj> list, 
            Func<TListedObj, bool> where, TListedObj obj)
        {
            IEnumerable<TListedObj> temp = list;
            if (where != null) temp = temp.Where(where);
            temp = temp.OrderByDescending(lo => GetScoreOfObject(lo));
            int i = 1;
            foreach (TListedObj objLoop in temp)
            {
                if (score >= GetScoreOfObject(objLoop))
                {
                    return i;
                }
                if (obj == null || !obj.Equals(objLoop)) i++;
            }
            return i;
        }

        public override void ValidateListedObject(TListedObj obj)
        {
            base.ValidateListedObject(obj);
            ValidateScore(obj.FinalScoreManual);
        }
    }
}
