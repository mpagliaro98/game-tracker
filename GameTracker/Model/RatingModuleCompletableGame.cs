﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;

namespace GameTracker.Model
{
    public abstract class RatingModuleCompletableGame<TRatableObj, TRatingCat, TCompStatus>
        : RatingModuleCompletable<TRatableObj, TRatingCat, TCompStatus>
        where TRatableObj : RatableGame, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
        where TCompStatus : CompletionStatusGame, ISavable, new()
    {
        protected IEnumerable<Platform> platforms;
        protected LoadSaveEngineGame<TRatableObj, TRatingCat, TCompStatus> loadSaveEngine;

        public IEnumerable<Platform> Platforms
        {
            get { return platforms; }
        }

        public override void Init()
        {
            base.Init();
            LoadPlatforms();
        }

        protected override void LoadCompletionStatuses()
        {
            completionStatuses = loadSaveEngine.LoadCompletionStatuses(this);
        }

        protected void LoadPlatforms()
        {
            platforms = loadSaveEngine.LoadPlatforms(this);
        }

        protected override void LoadRatableObjects()
        {
            ratableObjects = loadSaveEngine.LoadRatableObjects(this);
        }

        protected override void LoadRatingCategories()
        {
            ratingCategories = loadSaveEngine.LoadRatingCategories(this);
        }

        protected override void LoadScoreRanges()
        {
            scoreRanges = loadSaveEngine.LoadScoreRanges(this);
        }

        protected override void LoadSettings()
        {
            settings = loadSaveEngine.LoadSettings(this);
        }

        public override void SaveCompletionStatuses()
        {
            loadSaveEngine.SaveCompletionStatuses(completionStatuses);
        }

        public void SavePlatforms()
        {
            loadSaveEngine.SavePlatforms(platforms);
        }

        public override void SaveRatableObjects()
        {
            loadSaveEngine.SaveRatableObjects(ratableObjects);
        }

        public override void SaveRatingCategories()
        {
            loadSaveEngine.SaveRatingCategories(ratingCategories);
        }

        public override void SaveScoreRanges()
        {
            loadSaveEngine.SaveScoreRanges(scoreRanges);
        }

        public override void SaveSettings()
        {
            loadSaveEngine.SaveSettings(settings);
        }

        public Platform FindPlatform(ObjectReference objectKey)
        {
            return FindObject(platforms, objectKey);
        }

        public void AddPlatform(Platform obj)
        {
            AddToList(ref platforms, SavePlatforms, obj);
        }

        public void UpdatePlatform(Platform obj, Platform orig)
        {
            UpdateInList(ref platforms, SavePlatforms, obj, orig);
        }

        public void DeletePlatform(Platform obj)
        {
            DeleteFromList(ref platforms, SavePlatforms, obj);
            ratableObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatform());
            ratableObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatformPlayedOn.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatformPlayedOn());
            if (GlobalSettings.Autosave) SaveRatableObjects();
        }

        public IEnumerable<TRatableObj> GetGamesOnPlatform(Platform platform)
        {
            return RatableObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform));
        }

        public int GetNumGamesByPlatform(Platform platform)
        {
            return GetGamesOnPlatform(platform).Count();
        }

        public int GetNumGamesFinishableByPlatform(Platform platform)
        {
            return 0;
        }

        public double GetAverageScoreOfGamesByPlatform(Platform platform)
        {
            return 0;
        }

        public double GetHighestScoreFromGamesByPlatform(Platform platform)
        {
            return 0;
        }

        public double GetLowestScoreFromGamesByPlatform(Platform platform)
        {
            return 0;
        }

        public double GetNumGamesFinishedByPlatform(Platform platform)
        {
            return 0;
        }

        public double GetPercentageGamesFinishedByPlatform(Platform platform)
        {
            int numFinishable = GetNumGamesFinishableByPlatform(platform);
            if (numFinishable <= 0) numFinishable = 1;
            return GetNumGamesFinishedByPlatform(platform) / numFinishable * 100;
        }
    }
}
