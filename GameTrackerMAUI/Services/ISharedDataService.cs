using GameTracker;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public interface ISharedDataService
    {
        GameModule Module { get; }
        SettingsGame Settings { get; }
        IFileHandler FileHandlerSaves { get; }
        ILoadSaveHandler<ILoadSaveMethodGame> LoadSave { get; }
        bool Loaded { get; }
        void ResetSharedObjects();
        void Load();
        Task LoadAsync();
    }
}
