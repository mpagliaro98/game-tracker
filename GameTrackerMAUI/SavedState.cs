using GameTracker;
using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameTrackerMAUI
{
    [Serializable]
    public class SavedState
    {
        public const string SAVEDSTATE_FILENAME = "savedstate.dat";

        public FilterGames FilterGames { get; set; } = new FilterGames();
        public SortGames SortGames { get; set; } = new SortGames();
        public FilterPlatforms FilterPlatforms { get; set; } = new FilterPlatforms();
        public SortPlatforms SortPlatforms { get; set; } = new SortPlatforms();

        private SavedState() { }

        public static SavedState LoadSavedState(IPathController pathController, GameModule module, SettingsGame settings, Logger logger)
        {
            TextReader reader = null;
            SavedState savedState = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SavedState));
                reader = new StreamReader(pathController.Combine(pathController.ApplicationDirectory(), SAVEDSTATE_FILENAME));
                savedState = (SavedState)serializer.Deserialize(reader);
            }
            catch (FileNotFoundException)
            {
                // first load, file does not exist yet
                savedState = new SavedState();
            }
            catch (InvalidOperationException ex)
            {
                logger.Log("Exception when loading saved state - " + ex.Message);
                savedState = new SavedState();
            }
            finally
            {
                reader?.Close();

                savedState.FilterGames.Module = module;
                savedState.FilterGames.Settings = settings;
                savedState.SortGames.Module = module;
                savedState.SortGames.Settings = settings;
                savedState.FilterPlatforms.Module = module;
                savedState.FilterPlatforms.Settings = settings;
                savedState.SortPlatforms.Module = module;
                savedState.SortPlatforms.Settings = settings;
            }
            return savedState;
        }

        public static void SaveSavedState(IPathController pathController, SavedState savedState)
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SavedState));
                writer = new StreamWriter(pathController.Combine(pathController.ApplicationDirectory(), SAVEDSTATE_FILENAME));
                serializer.Serialize(writer, savedState);
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
