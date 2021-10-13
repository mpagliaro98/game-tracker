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
        protected string saveDir = PathController.Combine(PathController.BaseDirectory(), DIRECTORY_SAVE);

        public override IEnumerable<Platform> LoadPlatforms(RatingModule parentModule)
        {
            return LoadISavableList<Platform>(PathController.Combine(saveDir, FILENAME_PLATFORMS), parentModule);
        }

        public override void SavePlatforms(IEnumerable<Platform> platforms)
        {
            SaveISavableList(platforms, PathController.Combine(saveDir, FILENAME_PLATFORMS));
        }

        public override Settings LoadSettings(RatingModule parentModule)
        {
            return LoadISavable<Settings>(PathController.Combine(saveDir, FILENAME_SETTINGS), parentModule);
        }

        public override void SaveSettings(Settings settings)
        {
            SaveISavable(settings, PathController.Combine(saveDir, FILENAME_SETTINGS));
        }

        public override IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule)
        {
            return LoadISavableList<CompletionStatus>(PathController.Combine(saveDir, FILENAME_STATUSES), parentModule);
        }

        public override void SaveCompletionStatuses(IEnumerable<CompletionStatus> completionStatuses)
        {
            SaveISavableList(completionStatuses, PathController.Combine(saveDir, FILENAME_STATUSES));
        }

        public override IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadISavableList<RatableGame>(PathController.Combine(saveDir, FILENAME_GAMES), parentModule);
        }

        public override IEnumerable<ScoreRange> LoadScoreRanges(RatingModule parentModule)
        {
            return LoadISavableList<ScoreRange>(PathController.Combine(saveDir, FILENAME_RANGES), parentModule);
        }

        public override IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule)
        {
            return LoadISavableList<RatingCategory>(PathController.Combine(saveDir, FILENAME_CATEGORIES), parentModule);
        }

        public override void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects)
        {
            SaveISavableList(ratableObjects, PathController.Combine(saveDir, FILENAME_GAMES));
        }

        public override void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges)
        {
            SaveISavableList(scoreRanges, PathController.Combine(saveDir, FILENAME_RANGES));
        }

        public override void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories)
        {
            SaveISavableList(ratingCategories, PathController.Combine(saveDir, FILENAME_CATEGORIES));
        }

        protected void SaveJSONToFile(string serialized, string filepath)
        {
            CreateFileIfDoesNotExist(filepath);
            PathController.WriteToFile(filepath, serialized);
        }

        protected virtual void SaveISavableList(IEnumerable<ISavable> list, string filepath)
        {
            string serialized = Util.CreateJSONArray(list);
            SaveJSONToFile(serialized, filepath);
        }

        protected virtual void SaveISavable(ISavable obj, string filepath)
        {
            SavableRepresentation sr = obj.LoadIntoRepresentation();
            string serialized = sr.ConvertToJSON();
            SaveJSONToFile(serialized, filepath);
        }

        protected virtual IEnumerable<T> LoadISavableList<T>(string filepath, RatingModule parentModule) where T : ISavable, new()
        {
            string serialized = ReadJSONFromFile(filepath);
            IEnumerable<T> result = LoadJSONArrayIntoObjects<T>(serialized);
            SetParentModule(result, parentModule);
            return result;
        }

        protected virtual T LoadISavable<T>(string filepath, RatingModule parentModule) where T : ISavable, new()
        {
            string serialized = ReadJSONFromFile(filepath);
            SavableRepresentation sr = LoadJSONIntoRepresentation<T>(serialized);
            T t = new T();
            t.RestoreFromRepresentation(sr);
            SetParentModule(t, parentModule);
            return t;
        }

        protected string ReadJSONFromFile(string filepath)
        {
            CreateFileIfDoesNotExist(filepath);
            return PathController.ReadFromFile(filepath);
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
                SavableRepresentation sr = LoadJSONIntoRepresentation<T>(jsonObj);
                T t = new T();
                t.RestoreFromRepresentation(sr);
                result = result.Append(t).ToList();
            }
            return result;
        }

        protected SavableRepresentation LoadJSONIntoRepresentation<T>(string json) where T : ISavable, new()
        {
            if (json == "")
            {
                return new SavableRepresentation();
            }
            if (!Util.IsValidJSONObject(json))
            {
                throw new Exception("LoadSaveEngineGameJson LoadJSONIntoRepresentation: object is not valid json: " + json);
            }
            SavableRepresentation sr = new SavableRepresentation();
            JObject root = JObject.Parse(json);
            foreach (KeyValuePair<string, JToken> node in root)
            {
                if (node.Value is JArray)
                {
                    IEnumerable<SavableRepresentation> srList = new LinkedList<SavableRepresentation>();
                    var objects = node.Value;
                    foreach (JObject childRoot in objects)
                    {
                        string jsonObj = childRoot.ToString();
                        SavableRepresentation srChild = LoadJSONIntoRepresentation<T>(jsonObj);
                        srList = srList.Append(srChild).ToList();
                    }
                    sr.SaveList(node.Key, srList);
                }
                else if (node.Value is JObject || node.Value is JValue)
                {
                    sr.SaveValue(node.Key, node.Value.ToString());
                }
            }
            return sr;
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
    }
}
