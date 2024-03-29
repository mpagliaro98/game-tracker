﻿using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public class RatableTrackerFactory
    {
        public RankedObject GetModelObject(string typeName, Settings settings, TrackerModule module)
        {
            RankedObject obj;
            try
            {
                obj = CreateModelObjectFromTypeName(typeName, settings, module);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual RankedObject CreateModelObjectFromTypeName(string typeName, Settings settings, TrackerModule module)
        {
            RankedObject obj = null;
            switch (typeName.ToLower())
            {
                case "rankedobject":
                    obj = new RankedObject(settings, module);
                    break;
                case "rankedobjectstatus":
                    obj = new RankedObjectStatus(settings, (TrackerModuleStatuses)module);
                    break;
                case "ratedobject":
                    obj = new RatedObject((SettingsScore)settings, (TrackerModuleScores)module);
                    break;
                case "ratedobjectstatus":
                    obj = new RatedObjectStatus((SettingsScore)settings, (TrackerModuleScoreStatuses)module);
                    break;
                case "ratedobjectcategorical":
                    obj = new RatedObjectCategorical((SettingsScore)settings, (TrackerModuleScoreCategorical)module);
                    break;
                case "ratedobjectstatuscategorical":
                    obj = new RatedObjectStatusCategorical((SettingsScore)settings, (TrackerModuleScoreStatusCategorical)module);
                    break;
            }
            return obj;
        }

        public Status GetStatus(string typeName, IModuleStatus module, Settings settings)
        {
            Status obj;
            try
            {
                obj = CreateStatusFromTypeName(typeName, module, settings);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual Status CreateStatusFromTypeName(string typeName, IModuleStatus module, Settings settings)
        {
            Status obj = null;
            switch (typeName.ToLower())
            {
                case "status":
                    obj = new Status(module, settings);
                    break;
            }
            return obj;
        }

        public RatingCategory GetRatingCategory(string typeName, IModuleCategorical module, SettingsScore settings)
        {
            RatingCategory obj;
            try
            {
                obj = CreateRatingCategoryFromTypeName(typeName, module, settings);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual RatingCategory CreateRatingCategoryFromTypeName(string typeName, IModuleCategorical module, SettingsScore settings)
        {
            RatingCategory obj = null;
            switch (typeName.ToLower())
            {
                case "ratingcategory":
                    obj = new RatingCategory(module, settings);
                    break;
                case "ratingcategoryweighted":
                    obj = new RatingCategoryWeighted(module, settings);
                    break;
            }
            return obj;
        }

        public ScoreRange GetScoreRange(string typeName, TrackerModuleScores module, SettingsScore settings)
        {
            ScoreRange obj;
            try
            {
                obj = CreateScoreRangeFromTypeName(typeName, module, settings);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }
        
        protected virtual ScoreRange CreateScoreRangeFromTypeName(string typeName, TrackerModuleScores module, SettingsScore settings)
        {
            ScoreRange obj = null;
            switch (typeName.ToLower())
            {
                case "scorerange":
                    obj = new ScoreRange(module, settings);
                    break;
            }
            return obj;
        }

        public Settings GetSettings(string typeName)
        {
            Settings obj;
            try
            {
                obj = CreateSettingsFromTypeName(typeName);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Incorrect parameters passed in for creating " + typeName + ": " + e.Message);
            }
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual Settings CreateSettingsFromTypeName(string typeName)
        {
            Settings obj = null;
            switch (typeName.ToLower())
            {
                case "settings":
                    obj = new Settings();
                    break;
                case "settingsscore":
                    obj = new SettingsScore();
                    break;
            }
            return obj;
        }
    }
}
