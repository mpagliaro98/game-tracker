using GameTracker;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public static class SharedDataService
    {
        private static GameModule _module = null;
        public static GameModule Module
        {
            get
            {
                if (_module is null)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    _module = new GameModule(LoadSave, App.Logger);
                    _module.LoadData(Settings);
                    sw.Stop();
                    Debug.WriteLine("Module loaded in " + sw.ElapsedMilliseconds.ToString() + "ms");
                }
                return _module;
            }
            set { _module = value; }
        }

        private static SettingsGame _settings = null;
        public static SettingsGame Settings
        {
            get
            {
                if (_settings == null)
                {
                    try
                    {
                        _settings = (SettingsGame)RatableTracker.Util.Settings.Load(LoadSave);
                    }
                    catch (NoDataFoundException)
                    {
                        // first load
                        _settings = new SettingsGame();
                    }
                }
                return _settings;
            }
            set { _settings = value; }
        }

        public static IPathController PathController => new PathControllerMobile();

        public static GameTrackerFactory Factory => new GameTrackerFactory();

        private static IFileHandler _fileHandlerSaves = null;
        public static IFileHandler FileHandlerSaves
        {
            get
            {
                if (FileHandlerAWSS3.KeyFileExists(PathController))
                    _fileHandlerSaves ??= new FileHandlerAWSS3(PathController);
                else
                    _fileHandlerSaves ??= new FileHandlerLocalAppData(PathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
                return _fileHandlerSaves;
            }
        }

        private static ILoadSaveHandler<ILoadSaveMethodGame> _loadSave = null;
        public static ILoadSaveHandler<ILoadSaveMethodGame> LoadSave
        {
            get
            {
                _loadSave ??= new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(FileHandlerSaves, Factory, App.Logger));
                return _loadSave;
            }
        }

        private static SavedState _savedState = null;
        public static SavedState SavedState
        {
            get
            {
                _savedState ??= SavedState.LoadSavedState(PathController, Module, Settings, App.Logger);
                return _savedState;
            }
        }

        public static void ResetSharedObjects()
        {
            _fileHandlerSaves = null;
            _loadSave = null;
            _settings = null;
            _module = null;
            App.Logger.Log("Shared objects reset");
        }
    }
}
