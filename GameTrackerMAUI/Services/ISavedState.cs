using GameTracker;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public interface ISavedState
    {
        FilterEngine FilterGames { get; set; }
        SortEngine SortGames { get; set; }
        FilterEngine FilterPlatforms { get; set; }
        SortEngine SortPlatforms { get; set; }
        List<FilterEngine> GameSavedSearches { get; set; }
        List<FilterEngine> PlatformSavedSearches { get; set; }
        bool ShowCompilations { get; set; }
        bool ShowDLC { get; set; }
        bool Loaded { get; set; }
        void Load(IPathController pathController, ILogger logger, GameModule module, SettingsGame settings);
        void Save(IPathController pathController);
        void Delete(IPathController pathController);
    }
}
