using GameTracker;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    // extension methods to interface with the depedency injection container
    // allows extra work like loading data before returning or doing async loads
    public static class SharedData
    {
        private static Mutex _sharedDataLoadLock = new();

        public static ISavedState GetSavedState(this IServiceProvider provider)
        {
            var savedState = provider.GetService<ISavedState>();
            if (!savedState.Loaded)
            {
                var sharedDataService = provider.GetSharedDataService();
                savedState.Load(provider.GetService<IPathController>(), provider.GetService<ILogger>(), sharedDataService.Module, sharedDataService.Settings);
            }
            return savedState;
        }

        public static async Task<ISavedState> GetSavedStateAsync(this IServiceProvider provider)
        {
            var savedState = provider.GetService<ISavedState>();
            if (!savedState.Loaded)
            {
                var sharedDataService = await provider.GetSharedDataServiceAsync();
                savedState.Load(provider.GetService<IPathController>(), provider.GetService<ILogger>(), sharedDataService.Module, sharedDataService.Settings);
            }
            return savedState;
        }

        public static IPathController GetPathController(this IServiceProvider provider)
        {
            return provider.GetService<IPathController>();
        }

        public static GameTrackerFactory GetFactory(this IServiceProvider provider)
        {
            return provider.GetService<GameTrackerFactory>();
        }

        public static ILogger GetLogger(this IServiceProvider provider)
        {
            return provider.GetService<ILogger>();
        }

        public static ISharedDataService GetSharedDataService(this IServiceProvider provider)
        {
            var sharedData = provider.GetService<ISharedDataService>();
            _sharedDataLoadLock.WaitOne();
            try
            {
                if (!sharedData.Loaded)
                {
                    sharedData.Load();
                }
            }
            finally
            {
                _sharedDataLoadLock.ReleaseMutex();
            }
            return sharedData;
        }

        public static async Task<ISharedDataService> GetSharedDataServiceAsync(this IServiceProvider provider)
        {
            var sharedData = provider.GetService<ISharedDataService>();
            _sharedDataLoadLock.WaitOne();
            try
            {
                if (!sharedData.Loaded)
                {
                    await sharedData.LoadAsync();
                }
            }
            finally
            {
                _sharedDataLoadLock.ReleaseMutex();
            }
            return sharedData;
        }
    }
}
