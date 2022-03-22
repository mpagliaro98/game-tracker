using RatableTracker.Framework;
using RatableTracker.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerMobile
{
    public static class SavedState
    {
        private const string SAVED_STATE_FILENAME = "saved_state.dat";
        private static string SAVED_STATE_PATH = PathController.Combine(PathController.BaseDirectory(), SAVED_STATE_FILENAME);

        private static bool showCompilations = false;
        public static bool ShowCompilations
        {
            get => showCompilations;
            set
            {
                showCompilations = value;
                SaveSavedState();
            }
        }

        private static int gameSortMode = -1;
        public static int GameSortMode
        {
            get => gameSortMode;
            set
            {
                gameSortMode = value;
                SaveSavedState();
            }
        }

        private static SortMode gameSortDirection = SortMode.ASCENDING;
        public static SortMode GameSortDirection
        {
            get => gameSortDirection;
            set
            {
                gameSortDirection = value;
                SaveSavedState();
            }
        }

        public static void LoadSavedState()
        {
            PathController.CreateFileIfDoesNotExist(SAVED_STATE_PATH);
            var contents = PathController.ReadFromFile(SAVED_STATE_PATH);
            if (contents == "")
                return;
            string[] lines = contents.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        showCompilations = Convert.ToBoolean(lines[i]);
                        break;
                    case 1:
                        gameSortMode = Convert.ToInt32(lines[i]);
                        break;
                    case 2:
                        gameSortDirection = lines[i].ToUpper() == "DESCENDING" ? SortMode.DESCENDING : SortMode.ASCENDING;
                        break;
                }
            }
        }

        public static void SaveSavedState()
        {
            List<string> lines = new List<string>();
            lines.Add(showCompilations.ToString());
            lines.Add(gameSortMode.ToString());
            lines.Add(gameSortDirection.ToString());
            string contents = string.Join("\n", lines);
            PathController.WriteToFile(SAVED_STATE_PATH, contents);
        }
    }
}
