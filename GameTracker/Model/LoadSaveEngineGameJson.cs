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
        protected const string FILENAME_COMPILATIONS = "compilations.json";
        protected readonly IDictionary<LoadSaveIdentifier, string> filepathMap;
        protected static IEnumerable<string> LIST_OF_FILENAMES = new List<string>() { FILENAME_CATEGORIES, FILENAME_GAMES,
            FILENAME_PLATFORMS, FILENAME_RANGES, FILENAME_SETTINGS, FILENAME_STATUSES, FILENAME_COMPILATIONS };

        public IContentLoadSave<string, string> ContentLoadSaveInstance { get; set; } = new ContentLoadSaveLocal();

        public LoadSaveEngineGameJson()
        {
            filepathMap = new Dictionary<LoadSaveIdentifier, string>()
            {
                { ID_LISTEDOBJECTS, FILENAME_GAMES },
                { ID_PLATFORMS, FILENAME_PLATFORMS },
                { ID_STATUSES, FILENAME_STATUSES },
                { ID_RATINGCATEGORIES, FILENAME_CATEGORIES },
                { ID_RANGES, FILENAME_RANGES },
                { ID_SETTINGS, FILENAME_SETTINGS },
                { ID_COMPILATIONS, FILENAME_COMPILATIONS }
            };
        }

        public override IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = ContentLoadSaveInstance.Read(filename);
            IEnumerable<T> result = LoadJSONArrayIntoObjects<T>(serialized);
            return result;
        }

        public override async Task<IEnumerable<T>> LoadISavableListAsync<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = await ContentLoadSaveInstance.ReadAsync(filename);
            IEnumerable<T> result = LoadJSONArrayIntoObjects<T>(serialized);
            return result;
        }

        public override T LoadISavable<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = ContentLoadSaveInstance.Read(filename);
            SavableRepresentation<TValCont> sr = SavableRepresentation<TValCont>.LoadFromJSON(serialized);
            T t = new T();
            if (sr != null) t.RestoreFromRepresentation(sr);
            return t;
        }

        public override async Task<T> LoadISavableAsync<T>(LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = await ContentLoadSaveInstance.ReadAsync(filename);
            SavableRepresentation<TValCont> sr = SavableRepresentation<TValCont>.LoadFromJSON(serialized);
            T t = new T();
            if (sr != null) t.RestoreFromRepresentation(sr);
            return t;
        }

        public override void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = Util.CreateJSONArray<TValCont>(list.Cast<ISavable>());
            ContentLoadSaveInstance.Write(filename, serialized);
        }

        public override async Task SaveISavableListAsync<T>(IEnumerable<T> list, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            string serialized = Util.CreateJSONArray<TValCont>(list.Cast<ISavable>());
            await ContentLoadSaveInstance.WriteAsync(filename, serialized);
        }

        public override void SaveISavable<T>(T obj, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            SavableRepresentation<TValCont> sr = obj.LoadIntoRepresentation<TValCont>();
            string serialized = sr.ConvertToJSON();
            ContentLoadSaveInstance.Write(filename, serialized);
        }

        public override async Task SaveISavableAsync<T>(T obj, LoadSaveIdentifier id)
        {
            string filename = GetFilename(id);
            SavableRepresentation<TValCont> sr = obj.LoadIntoRepresentation<TValCont>();
            string serialized = sr.ConvertToJSON();
            await ContentLoadSaveInstance.WriteAsync(filename, serialized);
        }

        public override void TransferSaveFiles(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to)
        {
            ContentLoadSaveTransferString transfer = new ContentLoadSaveTransferString();
            transfer.TransferSaveFiles(from, to, LIST_OF_FILENAMES);
        }

        public override async Task TransferSaveFilesAsync(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to)
        {
            ContentLoadSaveTransferString transfer = new ContentLoadSaveTransferString();
            await transfer.TransferSaveFilesAsync(from, to, LIST_OF_FILENAMES);
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
