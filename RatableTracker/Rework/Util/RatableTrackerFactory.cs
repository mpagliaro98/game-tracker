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
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            throw new ArgumentException("Unknown type: " + typeName);
        }

        public virtual Status GetStatus(string typeName, StatusExtensionModule module)
        {
            switch (typeName.ToLower())
            {
                case "status":
                    return new Status(module);
            }
            throw new ArgumentException("Unknown type: " + typeName);
        }

        public virtual RatingCategory GetRatingCategory(string typeName, CategoryExtensionModule module)
        {
            switch (typeName.ToLower())
            {
                case "ratingcategory":
                    return new RatingCategory(module);
                case "ratingcategoryweighted":
                    return new RatingCategoryWeighted(module);
            }
            throw new ArgumentException("Unknown type: " + typeName);
        }

        public virtual ScoreRange GetScoreRange(string typeName, TrackerModuleScores module)
        {
            switch (typeName.ToLower())
            {
                case "scorerange":
                    return new ScoreRange(module);
            }
            throw new ArgumentException("Unknown type: " + typeName);
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
            throw new ArgumentException("Unknown type: " + typeName);
        }
    }
}
