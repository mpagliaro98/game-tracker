using GameTracker;
using GameTracker.Sorting;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
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

        public FilterEngine FilterGames { get; set; } = new FilterEngine();
        public SortEngine SortGames { get; set; } = new SortEngine();
        public FilterEngine FilterPlatforms { get; set; } = new FilterEngine();
        public SortEngine SortPlatforms { get; set; } = new SortEngine();
        public bool ShowCompilations { get; set; } = false;

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

                savedState.FilterGames.SetNonSerializableFields(module, settings);
                savedState.SortGames.SetNonSerializableFields(module, settings, new SortOptionModelName());
                savedState.FilterPlatforms.SetNonSerializableFields(module, settings);
                savedState.SortPlatforms.SetNonSerializableFields(module, settings, new SortOptionPlatformName());
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
