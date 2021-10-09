﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class RatingModuleGame : RatingModuleCompletable
    {
        protected IEnumerable<Platform> platforms;
        protected LoadSaveEngineGame loadSaveEngine;

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
    }
}
