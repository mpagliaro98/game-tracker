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

        public virtual void SetMinScoreAndUpdate(double newVal)
        {
            double oldMinScore = Settings.MinScore;
            Settings.MinScore = newVal;
            RecalculateScores(oldMinScore, Settings.MaxScore, Settings.MinScore, Settings.MaxScore);
            if (GlobalSettings.Autosave)
            {
                SaveSettings();
                SaveListedObjects();
            }
        }

        public virtual void SetMaxScoreAndUpdate(double newVal)
        {
            double oldMaxScore = Settings.MaxScore;
            Settings.MaxScore = newVal;
            RecalculateScores(Settings.MinScore, oldMaxScore, Settings.MinScore, Settings.MaxScore);
            if (GlobalSettings.Autosave)
            {
                SaveSettings();
                SaveListedObjects();
            }
        }

        public virtual bool ValidateManualScore(double val)
        {
            return !(val < Settings.MinScore || val > Settings.MaxScore);
        }

        public virtual void SetManualScoreAndBoundsCheck(TListedObj obj, double val)
        {
            if (!ValidateManualScore(val))
                throw new ScoreOutOfRangeException();
            obj.FinalScoreManual = val;
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
    }
}
