﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.Exceptions;

namespace GameTracker.Model
{
    public abstract class RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : CompletionStatus, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        protected IEnumerable<Platform> platforms;
        protected LoadSaveEngineGame<TListedObj, TRange, TSettings, TStatus, TRatingCat> loadSaveEngine;

        public IEnumerable<Platform> Platforms => platforms;

        public virtual int LimitPlatforms => 1000;

        public override void Init()
        {
            LoadPlatforms();
            base.Init();
        }

        public override void LoadStatuses()
        {
            statuses = loadSaveEngine.LoadStatuses();
        }

        public void LoadPlatforms()
        {
            platforms = loadSaveEngine.LoadPlatforms();
        }

        public override void LoadListedObjects()
        {
            listedObjs = loadSaveEngine.LoadListedObjects();
        }

        public override void LoadRatingCategories()
        {
            ratingCategories = loadSaveEngine.LoadRatingCategories();
        }

        public override void LoadRanges()
        {
            ranges = loadSaveEngine.LoadRanges();
        }

        public override void LoadSettings()
        {
            settings = loadSaveEngine.LoadSettings();
        }

        public override void SaveStatuses()
        {
            loadSaveEngine.SaveStatuses(statuses);
        }

        public void SavePlatforms()
        {
            loadSaveEngine.SavePlatforms(platforms);
        }

        public override void SaveListedObjects()
        {
            loadSaveEngine.SaveListedObjects(listedObjs);
        }

        public override void SaveRatingCategories()
        {
            loadSaveEngine.SaveRatingCategories(ratingCategories);
        }

        public override void SaveRanges()
        {
            loadSaveEngine.SaveRanges(ranges);
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
            ValidatePlatform(obj);
            AddToList(ref platforms, SavePlatforms, obj);
        }

        public void UpdatePlatform(Platform obj, Platform orig)
        {
            ValidatePlatform(obj);
            UpdateInList(ref platforms, SavePlatforms, obj, orig);
        }

        public void DeletePlatform(Platform obj)
        {
            DeleteFromList(ref platforms, SavePlatforms, obj);
            listedObjs.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatform());
            listedObjs.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatformPlayedOn.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatformPlayedOn());
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public IEnumerable<Platform> SortPlatforms<TField>(Func<Platform, TField> keySelector, SortMode mode = SortMode.ASCENDING)
        {
            return SortList(platforms, keySelector, mode);
        }

        public override void ValidateListedObject(TListedObj obj)
        {
            base.ValidateListedObject(obj);
            if (obj.CompletionCriteria.Length > RatableGame.MaxLengthCompletionCriteria)
                throw new ValidationException("Completion criteria cannot be longer than " + RatableGame.MaxLengthCompletionCriteria.ToString());
            if (obj.CompletionComment.Length > RatableGame.MaxLengthCompletionComment)
                throw new ValidationException("Completion comment cannot be longer than " + RatableGame.MaxLengthCompletionComment.ToString());
            if (obj.TimeSpent.Length > RatableGame.MaxLengthTimeSpent)
                throw new ValidationException("Time spent cannot be longer than " + RatableGame.MaxLengthTimeSpent.ToString());
        }

        public virtual void ValidatePlatform(Platform obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > Platform.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + Platform.MaxLengthName.ToString());
        }

        public IEnumerable<TListedObj> GetGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform));
        }

        public IEnumerable<TListedObj> GetFinishableGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform)
                && ((ro.RefStatus.HasReference() && !FindStatus(ro.RefStatus).ExcludeFromStats) || !ro.RefStatus.HasReference()));
        }

        public IEnumerable<TListedObj> GetFinishedGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform))
                .Where(ro => ro.RefStatus.HasReference() && FindStatus(ro.RefStatus).UseAsFinished);
        }

        public IEnumerable<TListedObj> GetFinishedGamesExcludeStatsOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform))
                .Where(ro => ro.RefStatus.HasReference() && FindStatus(ro.RefStatus).UseAsFinished && !FindStatus(ro.RefStatus).ExcludeFromStats);
        }

        public int GetNumGamesByPlatform(Platform platform)
        {
            return GetGamesOnPlatform(platform).Count();
        }

        public int GetNumGamesFinishableByPlatform(Platform platform)
        {
            return GetFinishableGamesOnPlatform(platform).Count();
        }

        public double GetAverageScoreOfGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Average(ro => GetScoreOfObject(ro));
        }

        public double GetHighestScoreFromGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Max(ro => GetScoreOfObject(ro));
        }

        public double GetLowestScoreFromGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Min(ro => GetScoreOfObject(ro));
        }

        public double GetNumGamesFinishedByPlatform(Platform platform)
        {
            return GetFinishedGamesOnPlatform(platform)
                .Where(game => !FindStatus(game.RefStatus).ExcludeFromStats)
                .Count();
        }

        public double GetPercentageGamesFinishedByPlatform(Platform platform)
        {
            int numFinishable = GetNumGamesFinishableByPlatform(platform);
            if (numFinishable <= 0) numFinishable = 1;
            return GetNumGamesFinishedByPlatform(platform) / numFinishable * 100;
        }

        public IEnumerable<TListedObj> GetTopGamesByPlatform(Platform platform, int numToGet)
        {
            return GetFinishedGamesOnPlatform(platform)
                .OrderByDescending(ro => GetScoreOfObject(ro))
                .Take(numToGet);
        }

        public IEnumerable<TListedObj> GetBottomGamesByPlatform(Platform platform, int numToGet)
        {
            return GetFinishedGamesOnPlatform(platform)
                .OrderBy(ro => GetScoreOfObject(ro))
                .Take(numToGet);
        }

        public int GetRankOfScoreByPlatform(double score, Platform platform)
        {
            return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), null);
        }

        public int GetRankOfScoreByPlatform(double score, Platform platform, TListedObj obj)
        {
            return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), obj);
        }
    }
}
