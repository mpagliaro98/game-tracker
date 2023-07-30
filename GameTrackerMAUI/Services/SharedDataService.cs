using GameTracker;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public class SharedDataService : ISharedDataService
    {
        public GameModule Module { get; private set; } = null;
        public SettingsGame Settings { get; private set; } = null;
        public IFileHandler FileHandlerSaves { get; private set; } = null;
        public ILoadSaveHandler<ILoadSaveMethodGame> LoadSave { get; private set; } = null;
        public bool Loaded { get; private set; } = false;

        private readonly GameTrackerFactory _factory;
        private readonly IPathController _pathController;
        private readonly ILogger _logger;

        public SharedDataService(IServiceProvider provider)
        {
            Loaded = false;
            _factory = provider.GetFactory();
            _pathController = provider.GetPathController();
            _logger = provider.GetLogger();
        }

        public void ResetSharedObjects()
        {
            Loaded = false;
            _logger.Log("Shared objects reset");
        }

        public void Load()
        {
            if (FileHandlerAWSS3.KeyFileExists(_pathController))
                FileHandlerSaves = new FileHandlerAWSS3(_pathController);
            else
                FileHandlerSaves = new FileHandlerLocalAppData(_pathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);

            LoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(FileHandlerSaves, _factory, new Logger(_logger)));

            try
            {
                Settings = (SettingsGame)RatableTracker.Util.Settings.Load(LoadSave);
            }
            catch (NoDataFoundException)
            {
                // first load
                Settings = new SettingsGame();
            }

            var sw = new Stopwatch();
            sw.Start();
            Module = new GameModule(LoadSave, new Logger(_logger));
            Module.LoadData(Settings);
            sw.Stop();
            Debug.WriteLine("Module loaded in " + sw.ElapsedMilliseconds.ToString() + "ms");

            Loaded = true;
        }

        public async Task LoadAsync()
        {
            if (FileHandlerAWSS3.KeyFileExists(_pathController))
                FileHandlerSaves = new FileHandlerAWSS3(_pathController);
            else
                FileHandlerSaves = new FileHandlerLocalAppData(_pathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);

            LoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(FileHandlerSaves, _factory, new Logger(_logger)));

            try
            {
                Settings = (SettingsGame)RatableTracker.Util.Settings.Load(LoadSave);
            }
            catch (NoDataFoundException)
            {
                // first load
                Settings = new SettingsGame();
            }

            var sw = new Stopwatch();
            sw.Start();
            Module = new GameModule(LoadSave, new Logger(_logger));
            await Module.LoadDataAsync(Settings);
            sw.Stop();
            Debug.WriteLine("Module loaded in " + sw.ElapsedMilliseconds.ToString() + "ms");

            Loaded = true;
        }
    }
}
