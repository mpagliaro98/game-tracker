using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public class RatableTrackerFactory
    {
        public virtual RankedObject GetModelObject(string typeName, Settings settings, TrackerModule module)
        {
            try
            {
                switch (typeName.ToLower())
                {
                    case "rankedobject":
                        return new RankedObject(settings, module);
                    case "rankedobjectstatus":
                        return new RankedObjectStatus(settings, (TrackerModuleStatuses)module);
                    case "ratedobject":
                        return new RatedObject((SettingsScore)settings, (TrackerModuleScores)module);
                    case "ratedobjectstatus":
                        return new RatedObjectStatus((SettingsScore)settings, (TrackerModuleScoreStatuses)module);
                    case "ratedobjectcategorical":
                        return new RatedObjectCategorical((SettingsScore)settings, (TrackerModuleScoreCategorical)module);
                    case "ratedobjectstatuscategorical":
                        return new RatedObjectStatusCategorical((SettingsScore)settings, (TrackerModuleScoreStatusCategorical)module);
                }
            }
            catch (InvalidCastException e)
            {
                // TODO throw unique exception (different from below)
                throw new Exception("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            // TODO throw unique exception
            throw new Exception("Unknown type: " + typeName);
        }

        public virtual Status GetStatus(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "status":
                    return new Status();
            }
            // TODO throw unique exception
            throw new Exception("Unknown type: " + typeName);
        }

        public virtual RatingCategory GetRatingCategory(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "ratingcategory":
                    return new RatingCategory();
                case "ratingcategoryweighted":
                    return new RatingCategoryWeighted();
            }
            // TODO throw unique exception
            throw new Exception("Unknown type: " + typeName);
        }

        public virtual ScoreRange GetScoreRange(string typeName, TrackerModuleScores module)
        {
            switch (typeName.ToLower())
            {
                case "scorerange":
                    return new ScoreRange(module);
            }
            // TODO throw unique exception
            throw new Exception("Unknown type: " + typeName);
        }

        public virtual Settings GetSettings(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "settings":
                    return new Settings();
                case "settingsscore":
                    return new SettingsScore();
            }
            // TODO throw unique exception
            throw new Exception("Unknown type: " + typeName);
        }
    }
}
