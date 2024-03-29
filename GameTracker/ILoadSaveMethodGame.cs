﻿using RatableTracker.Interfaces;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public interface ILoadSaveMethodGame : ILoadSaveMethodScoreStatusCategorical, ISaveBackupHandler
    {
        void SaveOnePlatform(Platform platform);
        void SaveAllPlatforms(IList<Platform> platform);
        void DeleteOnePlatform(Platform platform);
        IList<Platform> LoadPlatforms(GameModule module, SettingsGame settings);
        IList<Platform> LoadPlatformsAndFilter(GameModule module, SettingsGame settings, FilterEngine filterEngine, SortEngine sortEngine);
    }
}
