using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;

namespace GameTracker.Model
{
    public class RatingModuleGame : RatingModuleCompletable
    {
        protected IEnumerable<Platform> platforms;
        protected LoadSaveEngineGame loadSaveEngine;

        public IEnumerable<Platform> Platforms
        {
            get { return platforms; }
        }

        public RatingModuleGame(LoadSaveEngineGame loadSaveEngine)
        {
            this.loadSaveEngine = loadSaveEngine;
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
            ratableObjects.Cast<RatableGame>()
                .Where(ro => ro.Platform.Equals(obj))
                .ForEach(ro => ro.RemovePlatform());
            ratableObjects.Cast<RatableGame>()
                .Where(ro => ro.PlatformPlayedOn.Equals(obj))
                .ForEach(ro => ro.RemovePlatformPlayedOn());
            if (GlobalSettings.Autosave) SaveRatableObjects();
        }
    }
}
