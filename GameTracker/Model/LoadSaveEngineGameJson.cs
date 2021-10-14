using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RatableTracker.Framework;
using RatableTracker.Framework.PathController;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;

namespace GameTracker.Model
{
    public class LoadSaveEngineGameJson : LoadSaveEngineGame
    {
        protected const string FILENAME_PLATFORMS = "platforms.json";
        protected const string FILENAME_GAMES = "games.json";
        protected const string FILENAME_STATUSES = "completion_statuses.json";
        protected const string FILENAME_CATEGORIES = "rating_categories.json";
        protected const string FILENAME_RANGES = "score_ranges.json";
        protected const string FILENAME_SETTINGS = "settings.json";
        protected const string DIRECTORY_SAVE = "savefiles";
        protected static string SAVE_DIR = PathController.Combine(PathController.BaseDirectory(), DIRECTORY_SAVE);
        protected readonly IDictionary<Type, string> filepathMap = new Dictionary<Type, string>()
        {
            { typeof(RatableGame), PathController.Combine(SAVE_DIR, FILENAME_GAMES) },
            { typeof(Platform), PathController.Combine(SAVE_DIR, FILENAME_PLATFORMS) },
            { typeof(CompletionStatus), PathController.Combine(SAVE_DIR, FILENAME_STATUSES) },
            { typeof(RatingCategoryWeighted), PathController.Combine(SAVE_DIR, FILENAME_CATEGORIES) },
            { typeof(ScoreRange), PathController.Combine(SAVE_DIR, FILENAME_RANGES) },
            { typeof(Settings), PathController.Combine(SAVE_DIR, FILENAME_SETTINGS) }
        };

        protected override IEnumerable<T> LoadISavableList<T>()
        {
            string filepath = GetFilenameForType(typeof(T));
            string serialized = ReadJSONFromFile(filepath);
            IEnumerable<T> result = LoadJSONArrayIntoObjects<T>(serialized);
            return result;
        }

        protected override T LoadISavable<T>()
        {
            string filepath = GetFilenameForType(typeof(T));
            string serialized = ReadJSONFromFile(filepath);
            SavableRepresentation sr = SavableRepresentation.LoadFromJSON(serialized);
            T t = new T();
            t.RestoreFromRepresentation(sr);
            return t;
        }

        protected override void SaveISavableList<T>(IEnumerable<T> list)
        {
            string filepath = GetFilenameForType(typeof(T));
            string serialized = Util.CreateJSONArray(list.Cast<ISavable>());
            SaveJSONToFile(serialized, filepath);
        }

        protected override void SaveISavable<T>(T obj)
        {
            string filepath = GetFilenameForType(typeof(T));
            SavableRepresentation sr = obj.LoadIntoRepresentation();
            string serialized = sr.ConvertToJSON();
            SaveJSONToFile(serialized, filepath);
        }

        protected string ReadJSONFromFile(string filepath)
        {
            CreateFileIfDoesNotExist(filepath);
            return PathController.ReadFromFile(filepath);
        }

        protected void SaveJSONToFile(string serialized, string filepath)
        {
            CreateFileIfDoesNotExist(filepath);
            PathController.WriteToFile(filepath, serialized);
        }

        protected IEnumerable<T> LoadJSONArrayIntoObjects<T>(string json) where T : ISavable, new()
        {
            if (json == "")
            {
                return new LinkedList<T>();
            }
            if (!Util.IsValidJSONArray(json))
            {
                throw new Exception("LoadSaveEngineGameJson LoadJSONArrayIntoObjects: object is not valid json: " + json);
            }
            IEnumerable<T> result = new LinkedList<T>();
            var objects = JArray.Parse(json);
            foreach (JObject root in objects)
            {
                string jsonObj = root.ToString();
                SavableRepresentation sr = SavableRepresentation.LoadFromJSON(jsonObj);
                T t = new T();
                t.RestoreFromRepresentation(sr);
                result = result.Append(t).ToList();
            }
            return result;
        }
        
        protected void CreateFileIfDoesNotExist(string filepath)
        {
            if (!PathController.FileExists(filepath))
            {
                string directory = PathController.GetDirectoryFromFilename(filepath);
                PathController.CreateDirectory(directory);
                System.IO.FileStream fs = PathController.CreateFile(filepath);
                fs.Close();
            }
        }

        protected string GetFilenameForType(Type type)
        {
            if (!filepathMap.ContainsKey(type))
            {
                throw new Exception("Attempting to get filename of type " + type.ToString() + ", which is not handled");
            }
            return filepathMap[type];
        }
    }
}
