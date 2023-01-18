﻿using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveMethodJSON : ILoadSaveMethodScoreStatusCategorical
    {
        protected const string SAVE_FILE_DIRECTORY = "savefiles";

        protected const string MODEL_OBJECT_FILE = "modelobjects.json";
        protected const string SETTINGS_FILE = "settings.json";
        protected const string SCORE_RANGE_FILE = "scoreranges.json";
        protected const string STATUS_FILE = "statuses.json";
        protected const string CATEGORY_FILE = "ratingcategories.json";

        protected IList<SavableRepresentation> modelObjects = null;
        protected SavableRepresentation settings = null;
        protected IList<SavableRepresentation> scoreRanges = null;
        protected IList<SavableRepresentation> statuses = null;
        protected IList<SavableRepresentation> categories = null;

        protected readonly IFileHandler fileHandler;
        protected readonly IPathController pathController;
        protected readonly RatableTrackerFactory factory;

        public LoadSaveMethodJSON(IFileHandler fileHandler, IPathController pathController, RatableTrackerFactory factory)
        {
            this.fileHandler = fileHandler;
            this.pathController = pathController;
            this.factory = factory;
        }

        #region "Internal"
        protected void EnsureFileContentIsLoaded<T>(string fileName, ref T representation, Func<byte[], T> interpretBytesFromFile)
        {
            if (representation != null) return;
            string path = CreateRelativePath(fileName);
            byte[] fileContent = fileHandler.LoadFile(path);
            representation = interpretBytesFromFile(fileContent);
        }

        protected void EnsureModelObjectsAreLoaded()
        {
            EnsureFileContentIsLoaded(MODEL_OBJECT_FILE, ref modelObjects, InterpretBytesToSRList);
        }

        protected void EnsureSettingsAreLoaded()
        {
            EnsureFileContentIsLoaded(SETTINGS_FILE, ref settings, InterpretBytesToSR);
        }

        protected void EnsureScoreRangesAreLoaded()
        {
            EnsureFileContentIsLoaded(SCORE_RANGE_FILE, ref scoreRanges, InterpretBytesToSRList);
        }

        protected void EnsureStatusesAreLoaded()
        {
            EnsureFileContentIsLoaded(STATUS_FILE, ref statuses, InterpretBytesToSRList);
        }

        protected void EnsureCategoriesAreLoaded()
        {
            EnsureFileContentIsLoaded(CATEGORY_FILE, ref categories, InterpretBytesToSRList);
        }

        protected void SaveFileContentIfLoaded<T>(string fileName, T representation, Func<T, byte[]> representationToBytes)
        {
            if (representation == null) return;
            byte[] fileContent = representationToBytes(representation);
            string path = CreateRelativePath(fileName);
            fileHandler.SaveFile(path, fileContent);
        }

        protected void SaveModelObjectsIfLoaded()
        {
            SaveFileContentIfLoaded(MODEL_OBJECT_FILE, modelObjects, SRListToBytes);
        }

        protected void SaveSettingsIfLoaded()
        {
            SaveFileContentIfLoaded(SETTINGS_FILE, settings, SRToBytes);
        }

        protected void SaveScoreRangesIfLoaded()
        {
            SaveFileContentIfLoaded(SCORE_RANGE_FILE, modelObjects, SRListToBytes);
        }

        protected void SaveStatusesIfLoaded()
        {
            SaveFileContentIfLoaded(STATUS_FILE, statuses, SRListToBytes);
        }

        protected void SaveCategoriesIfLoaded()
        {
            SaveFileContentIfLoaded(CATEGORY_FILE, categories, SRListToBytes);
        }

        private IList<SavableRepresentation> InterpretBytesToSRList(byte[] bytes)
        {
            string json = Util.Util.TextEncoding.GetString(bytes);
            if (json == "")
            {
                return new List<SavableRepresentation>();
            }
            // TODO throw unique exception
            if (!IsValidJSONArray(json))
            {
                throw new Exception("LoadSaveMethodJSON InterpretBytesToSRList: object is not valid json: " + json);
            }
            IList<SavableRepresentation> result = new List<SavableRepresentation>();
            var objects = JArray.Parse(json);
            foreach (JObject root in objects.Cast<JObject>())
            {
                string jsonObj = root.ToString();
                SavableRepresentation sr = JSONToSavableRepresentation(jsonObj);
                result.Add(sr);
            }
            return result;
        }

        private SavableRepresentation InterpretBytesToSR(byte[] bytes)
        {
            string json = Util.Util.TextEncoding.GetString(bytes);
            return JSONToSavableRepresentation(json);
        }

        private byte[] SRListToBytes(IList<SavableRepresentation> representation)
        {
            string json = CreateJSONArray(representation);
            return Util.Util.TextEncoding.GetBytes(json);
        }

        private byte[] SRToBytes(SavableRepresentation representation)
        {
            string json = SavableRepresentationToJSON(representation);
            return Util.Util.TextEncoding.GetBytes(json);
        }

        protected string CreateRelativePath(string fileName)
        {
            return pathController.Combine(SAVE_FILE_DIRECTORY, fileName);
        }

        protected void SaveOne<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, T toSave, string keyName = "UniqueID") where T : SavableObject, IKeyable
        {
            ensureLoaded();
            for (int i = 0; i < data.Count; i++)
            {
                var sr = data[i];
                if (sr.GetValue(keyName).GetUniqueID().Equals(toSave.UniqueID))
                {
                    data[i] = toSave.LoadIntoRepresentation();
                    break;
                }
            }
        }

        protected void SaveAll<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, IList<T> toSave) where T : SavableObject, IKeyable
        {
            ensureLoaded();
            data = new List<SavableRepresentation>();
            foreach (var savable in toSave)
            {
                data.Add(savable.LoadIntoRepresentation());
            }
        }

        protected void DeleteOne<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, T toDelete, string keyName = "UniqueID") where T : SavableObject, IKeyable
        {
            ensureLoaded();
            for (int i = 0; i < data.Count; i++)
            {
                var sr = data[i];
                if (sr.GetValue(keyName).GetUniqueID().Equals(toDelete.UniqueID))
                {
                    data.RemoveAt(i);
                    break;
                }
            }
        }

        protected IList<T> LoadAll<T>(Action ensureLoaded, IList<SavableRepresentation> data, Func<string, T> generateObj, string keyTypeName = "TypeName") where T : SavableObject, IKeyable
        {
            return LoadAll<T, int>(ensureLoaded, data, generateObj, null, keyTypeName: keyTypeName);
        }

        protected IList<T> LoadAll<T, TSort>(Action ensureLoaded, IList<SavableRepresentation> data, Func<string, T> generateObj, Func<T, TSort> sortExpression, string keyTypeName = "TypeName") where T : SavableObject, IKeyable
        {
            ensureLoaded();
            IList<T> list = new List<T>();
            foreach (var sr in data)
            {
                string typeName = sr.GetValue(keyTypeName).ToString();
                T obj = generateObj(typeName);
                obj.RestoreFromRepresentation(sr);
                list.Add(obj);
            }
            if (sortExpression != null) list = list.OrderBy(sortExpression).ToList();
            return list;
        }

        public virtual void Dispose()
        {
            SaveModelObjectsIfLoaded();
            SaveSettingsIfLoaded();
            SaveScoreRangesIfLoaded();
            SaveStatusesIfLoaded();
            SaveCategoriesIfLoaded();
        }
        #endregion

        #region "JSON Util"
        public static string FixJSONSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"").Replace("\n", "\\n");
        }

        public static string CreateJSONArray(IEnumerable<SavableRepresentation> objs)
        {
            ICollection<string> jsonObjs = new LinkedList<string>();
            foreach (SavableRepresentation obj in objs)
            {
                string json = SavableRepresentationToJSON(obj);
                jsonObjs.Add(json);
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static string CreateJSONArray(IEnumerable<SavableObject> objs)
        {
            ICollection<string> jsonObjs = new LinkedList<string>();
            foreach (SavableObject obj in objs)
            {
                SavableRepresentation sr = obj.LoadIntoRepresentation();
                string json = SavableRepresentationToJSON(sr);
                jsonObjs.Add(json);
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static string CreateJSONArray(IEnumerable<string> objs)
        {
            ICollection<string> jsonObjs = new LinkedList<string>();
            foreach (string obj in objs)
            {
                string json = "\"" + FixJSONSpecialChars(obj) + "\"";
                jsonObjs.Add(json);
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static bool IsValidJSONObject(string json)
        {
            try
            {
                JToken root = JToken.Parse(json);
                return root is JObject;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidJSONArray(string json)
        {
            try
            {
                JToken root = JToken.Parse(json);
                return root is JArray;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string SavableRepresentationToJSON(SavableRepresentation sr)
        {
            string json = "";
            foreach (string key in sr.GetAllSavedKeys())
            {
                if (json != "") json += ",";
                string jsonObj = "\"" + FixJSONSpecialChars(key) + "\":" + ConvertValueToJSON(sr.GetValue(key));
                json += jsonObj;
            }
            json = "{" + json + "}";
            return json;
        }

        public static string ConvertValueToJSON(ValueContainer vc)
        {
            string jsonObj = "";
            if (vc.HasValue())
            {
                if (vc.IsValueAList())
                {
                    if (vc.IsValueObjectList())
                        jsonObj += CreateJSONArray(vc.GetSavableRepresentationList());
                    else
                        jsonObj += CreateJSONArray(vc.GetStringList());
                }
                else
                {
                    if (vc.IsValueObject())
                        jsonObj += SavableRepresentationToJSON(vc.GetSavableRepresentation());
                    else
                        jsonObj += "\"" + FixJSONSpecialChars(vc.GetString()) + "\"";
                }
            }
            else
            {
                jsonObj += "\"\"";
            }
            return jsonObj;
        }

        public static SavableRepresentation JSONToSavableRepresentation(string json)
        {
            if (json == "")
            {
                return new SavableRepresentation();
            }
            if (!IsValidJSONObject(json))
            {
                throw new Exception("LoadSaveMethodJSON JSONToSavableRepresentation: object is not valid json: " + json);
            }
            SavableRepresentation sr = new SavableRepresentation();
            JObject root = JObject.Parse(json);
            foreach (KeyValuePair<string, JToken> node in root)
            {
                if (node.Value is JArray)
                {
                    // Node is an array
                    if (((JArray)node.Value).First is JValue)
                    {
                        // Array is all single values (assumes all values are same type)
                        LinkedList<string> stringList = new LinkedList<string>();
                        var objects = node.Value;
                        foreach (JValue childRoot in objects)
                        {
                            stringList.AddLast(childRoot.Value.ToString());
                        }
                        sr.SaveValue(node.Key, new ValueContainer(stringList));
                    }
                    else if (((JArray)node.Value).First is JObject)
                    {
                        // Array is all json objects (assumes all values are same type)
                        LinkedList<SavableRepresentation> srList = new LinkedList<SavableRepresentation>();
                        var objects = node.Value;
                        foreach (JObject childRoot in objects)
                        {
                            string jsonObj = childRoot.ToString();
                            SavableRepresentation srChild = JSONToSavableRepresentation(jsonObj);
                            srList.AddLast(srChild);
                        }
                        sr.SaveValue(node.Key, new ValueContainer(srList));
                    }
                }
                else if (node.Value is JValue)
                {
                    // Node is a single value
                    sr.SaveValue(node.Key, new ValueContainer(node.Value.ToString()));
                }
                else if (node.Value is JObject)
                {
                    // Node is a json array
                    sr.SaveValue(node.Key, new ValueContainer(JSONToSavableRepresentation(node.Value.ToString())));
                }
            }
            return sr;
        }
        #endregion

        public void DeleteOneModelObject(RankedObject rankedObject)
        {
            DeleteOne(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObject);
        }

        public IList<RankedObject> LoadModelObjects(Settings settings, TrackerModule module)
        {
            return LoadAll(EnsureModelObjectsAreLoaded, modelObjects, (s) => factory.GetModelObject(s, settings, module), (obj) => obj.Rank);
        }

        public void SaveAllModelObjects(IList<RankedObject> rankedObjects)
        {
            SaveAll(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObjects);
        }

        public void SaveOneModelObject(RankedObject rankedObject)
        {
            SaveOne(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObject);
        }

        public void SaveOneScoreRange(ScoreRange scoreRange)
        {
            SaveOne(EnsureScoreRangesAreLoaded, ref scoreRanges, scoreRange);
        }

        public void SaveAllScoreRanges(IList<ScoreRange> scoreRanges)
        {
            SaveAll(EnsureScoreRangesAreLoaded, ref this.scoreRanges, scoreRanges);
        }

        public void DeleteOneScoreRange(ScoreRange scoreRange)
        {
            DeleteOne(EnsureScoreRangesAreLoaded, ref scoreRanges, scoreRange);
        }

        public IList<ScoreRange> LoadScoreRanges(TrackerModuleScores module)
        {
            return LoadAll(EnsureScoreRangesAreLoaded, scoreRanges, (s) => factory.GetScoreRange(s, module));
        }

        public void SaveSettings(Settings settings)
        {
            EnsureSettingsAreLoaded();
            this.settings = settings.LoadIntoRepresentation();
        }

        public Settings LoadSettings()
        {
            EnsureSettingsAreLoaded();
            string typeName = this.settings.GetValue("TypeName").GetString();
            Settings settings = factory.GetSettings(typeName);
            settings.RestoreFromRepresentation(this.settings);
            return settings;
        }

        public void SaveOneCategory(RatingCategory ratingCategory)
        {
            SaveOne(EnsureCategoriesAreLoaded, ref categories, ratingCategory);
        }

        public void SaveAllCategories(IList<RatingCategory> ratingCategories)
        {
            SaveAll(EnsureCategoriesAreLoaded, ref categories, ratingCategories);
        }

        public void DeleteOneCategory(RatingCategory ratingCategory)
        {
            DeleteOne(EnsureCategoriesAreLoaded, ref categories, ratingCategory);
        }

        public IList<RatingCategory> LoadCategories(CategoryExtensionModule module)
        {
            return LoadAll(EnsureCategoriesAreLoaded, categories, (s) => factory.GetRatingCategory(s, module));
        }

        public void SaveOneStatus(Status status)
        {
            SaveOne(EnsureStatusesAreLoaded, ref statuses, status);
        }

        public void SaveAllStatuses(IList<Status> statuses)
        {
            SaveAll(EnsureStatusesAreLoaded, ref this.statuses, statuses);
        }

        public void DeleteOneStatus(Status status)
        {
            DeleteOne(EnsureStatusesAreLoaded, ref statuses, status);
        }

        public IList<Status> LoadStatuses(StatusExtensionModule module)
        {
            return LoadAll(EnsureStatusesAreLoaded, statuses, (s) => factory.GetStatus(s, module));
        }
    }
}