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

            foreach (TListedObj ro in listedObjs)
            {
                ScaleScoreOfObject(ro, oldRange, newRange, minRangeOld, minRangeNew);
            }
        }

        protected virtual void ScaleScoreOfObject(TListedObj obj, double oldRange, double newRange, double minRangeOld, double minRangeNew)
        {
            if (oldRange == 0)
                obj.FinalScoreManual = minRangeNew;
            else
                obj.FinalScoreManual = ((GetScoreOfObject(obj) - minRangeOld) * newRange / oldRange) + minRangeNew;
        }

        public override System.Drawing.Color GetColorFromRange(TListedObj obj)
        {
            double score = GetScoreOfObject(obj);
            TRange range = ApplyRange(score);
            return range == null ? new System.Drawing.Color() : range.Color;
        }

        public virtual double GetScoreOfObject(TListedObj ro)
        {
            return ro.FinalScoreManual;
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
    }
}
