using GameTracker;
using GameTracker.Sorting;
using GameTrackerMAUI.Services;
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
    public class SavedState : ISavedState
    {
        public const string SAVEDSTATE_FILENAME = "savedstate.dat";

        public FilterEngine FilterGames { get; set; } = new FilterEngine();
        public SortEngine SortGames { get; set; } = new SortEngine();
        public FilterEngine FilterPlatforms { get; set; } = new FilterEngine();
        public SortEngine SortPlatforms { get; set; } = new SortEngine();
        public bool ShowCompilations { get; set; } = false;
        public bool Loaded { get; set; } = false;

        public SavedState() { }

        public void Load(IPathController pathController, ILogger logger, GameModule module, SettingsGame settings)
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
            }
            catch (InvalidOperationException ex)
            {
                logger.Log("Exception when loading saved state - " + ex.Message);
            }
            finally
            {
                reader?.Close();

                if (savedState != null)
                {
                    FilterGames = savedState.FilterGames;
                    FilterPlatforms = savedState.FilterPlatforms;
                    SortGames = savedState.SortGames;
                    SortPlatforms = savedState.SortPlatforms;
                    ShowCompilations = savedState.ShowCompilations;

                    FilterGames.SetNonSerializableFields(module, settings);
                    SortGames.SetNonSerializableFields(module, settings, new SortOptionModelName());
                    FilterPlatforms.SetNonSerializableFields(module, settings);
                    SortPlatforms.SetNonSerializableFields(module, settings, new SortOptionPlatformName());
                }

                Loaded = true;
            }
        }

        public void Save(IPathController pathController)
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SavedState));
                writer = new StreamWriter(pathController.Combine(pathController.ApplicationDirectory(), SAVEDSTATE_FILENAME));
                serializer.Serialize(writer, this);
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
