using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void Init()
        {
            loadSaveEngine = new LoadSaveEngineGameJson();
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

        public Platform FindPlatform(string name)
        {
            foreach (Platform p in platforms)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            throw new NameNotFoundException("RatingModuleGame FindPlatform: could not find name of " + name);
        }

        public void AddPlatform(Platform obj)
        {
            AddToList(ref platforms, obj);
        }

        public void UpdatePlatform(Platform obj, Platform orig)
        {
            UpdateInList(ref platforms, obj, orig);
        }
    }
}
