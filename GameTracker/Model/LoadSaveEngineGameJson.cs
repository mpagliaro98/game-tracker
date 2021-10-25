using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RatableTracker.Framework;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.IO;

namespace GameTracker.Model
{
    public class LoadSaveEngineGameJson<TValCont>
        : LoadSaveEngineGame<RatableGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
        where TValCont : IValueContainer<TValCont>, new()
    {
        protected const string FILENAME_PLATFORMS = "platforms.json";
        protected const string FILENAME_GAMES = "games.json";
        protected const string FILENAME_STATUSES = "completion_statuses.json";
        protected const string FILENAME_CATEGORIES = "rating_categories.json";
        protected const string FILENAME_RANGES = "score_ranges.json";
        protected const string FILENAME_SETTINGS = "settings.json";
        
        protected readonly IDictionary<LoadSaveIdentifier, string> filepathMap = new Dictionary<LoadSaveIdentifier, string>()
        {
            { ID_LISTEDOBJECTS, FILENAME_GAMES },
            { ID_PLATFORMS, FILENAME_PLATFORMS },
            { ID_STATUSES, FILENAME_STATUSES },
            { ID_RATINGCATEGORIES, FILENAME_CATEGORIES },
            { ID_RANGES, FILENAME_RANGES },
            { ID_SETTINGS, FILENAME_SETTINGS }
        };

        protected override IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = FileLoadSave.ReadStringFromFile(filename);
            IEnumerable<T> result = LoadJSONArrayIntoObjects<T>(serialized);
            return result;
        }

        protected override T LoadISavable<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = FileLoadSave.ReadStringFromFile(filename);
            SavableRepresentation<TValCont> sr = SavableRepresentation<TValCont>.LoadFromJSON(serialized);
            T t = new T();
            if (sr != null) t.RestoreFromRepresentation(sr);
            return t;
        }

        protected override void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = Util.CreateJSONArray<TValCont>(list.Cast<ISavable>());
            FileLoadSave.WriteStringToFile(filename, serialized);
        }

        protected override void SaveISavable<T>(T obj, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            SavableRepresentation<TValCont> sr = obj.LoadIntoRepresentation<TValCont>();
            string serialized = sr.ConvertToJSON();
            FileLoadSave.WriteStringToFile(filename, serialized);
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
                SavableRepresentation<TValCont> sr = SavableRepresentation<TValCont>.LoadFromJSON(jsonObj);
                T t = new T();
                if (sr != null) t.RestoreFromRepresentation(sr);
                result = result.Append(t).ToList();
            }
            return result;
        }
        
        protected string GetFilename(LoadSaveIdentifier id)
        {
            if (!filepathMap.ContainsKey(id))
            {
                throw new Exception("Attempting to get filename of ID " + id.ToString() + ", which is not handled");
            }
            return filepathMap[id];
        }
    }
}
