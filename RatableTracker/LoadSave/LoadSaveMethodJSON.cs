using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using RatableTracker.Exceptions;

namespace RatableTracker.LoadSave
{
    public class LoadSaveMethodJSON : ILoadSaveMethodScoreStatusCategorical
    {
        public const string SAVE_FILE_DIRECTORY = "savefiles";

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

        protected bool modelObjectsChanged = false;
        protected bool settingsChanged = false;
        protected bool scoreRangesChanged = false;
        protected bool statusesChanged = false;
        protected bool categoriesChanged = false;

        protected bool operationCanceled = false;

        protected readonly IFileHandler fileHandler;
        protected readonly RatableTrackerFactory factory;
        protected readonly Logger logger;

        public LoadSaveMethodJSON(IFileHandler fileHandler, RatableTrackerFactory factory) : this(fileHandler, factory, new Logger()) { }

        public LoadSaveMethodJSON(IFileHandler fileHandler, RatableTrackerFactory factory, Logger logger)
        {
            this.fileHandler = fileHandler;
            this.factory = factory;
            this.logger = logger;
            this.logger.Log(GetType().Name + " connection opened");
        }

        #region "Internal"
        protected void EnsureFileContentIsLoaded<T>(string fileName, ref T representation, Func<byte[], T> interpretBytesFromFile)
        {
            if (representation != null) return;
            logger.Log("Load started from file: " + fileName);
            Stopwatch sw = Stopwatch.StartNew();
            byte[] fileContent = fileHandler.LoadFile(fileName, logger);
            sw.Stop();
            logger.Log("Load from " + fileName + " finished in " + sw.ElapsedMilliseconds.ToString() + "ms (" + fileContent.Length.ToString() + " bytes)");
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

        protected void SaveFileContentIfLoaded<T>(string fileName, ref T representation, Func<T, byte[]> representationToBytes, bool changed)
        {
            if (representation == null) return;
            if (!changed) return;
            byte[] fileContent = representationToBytes(representation);
            logger.Log("Save started to file: " + fileName + " (" + fileContent.Length.ToString() + " bytes)");
            Stopwatch sw = Stopwatch.StartNew();
            fileHandler.SaveFile(fileName, fileContent, logger);
            sw.Stop();
            representation = default; // null
            logger.Log("Save to " + fileName + " finished in " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        protected void SaveModelObjectsIfLoaded()
        {
            SaveFileContentIfLoaded(MODEL_OBJECT_FILE, ref modelObjects, SRListToBytes, modelObjectsChanged);
        }

        protected void SaveSettingsIfLoaded()
        {
            SaveFileContentIfLoaded(SETTINGS_FILE, ref settings, SRToBytes, settingsChanged);
        }

        protected void SaveScoreRangesIfLoaded()
        {
            SaveFileContentIfLoaded(SCORE_RANGE_FILE, ref scoreRanges, SRListToBytes, scoreRangesChanged);
        }

        protected void SaveStatusesIfLoaded()
        {
            SaveFileContentIfLoaded(STATUS_FILE, ref statuses, SRListToBytes, statusesChanged);
        }

        protected void SaveCategoriesIfLoaded()
        {
            SaveFileContentIfLoaded(CATEGORY_FILE, ref categories, SRListToBytes, categoriesChanged);
        }

        protected IList<SavableRepresentation> InterpretBytesToSRList(byte[] bytes)
        {
            string json = Util.Util.TextEncoding.GetString(bytes);
            return JSONToSavableRepresentationList(json);
        }

        protected SavableRepresentation InterpretBytesToSR(byte[] bytes)
        {
            string json = Util.Util.TextEncoding.GetString(bytes);
            return JSONToSavableRepresentation(json);
        }

        protected byte[] SRListToBytes(IList<SavableRepresentation> representation)
        {
            string json = CreateJSONArray(representation);
            return Util.Util.TextEncoding.GetBytes(json);
        }

        protected byte[] SRToBytes(SavableRepresentation representation)
        {
            string json = SavableRepresentationToJSON(representation);
            return Util.Util.TextEncoding.GetBytes(json);
        }

        public void SaveOne<T>(Action ensureLoaded, ref SavableRepresentation data, T toSave, ref bool changed) where T : RepresentationObject
        {
            ensureLoaded();
            data = toSave.LoadIntoRepresentation();
            changed = true;
        }

        public T LoadOne<T>(Action ensureLoaded, ref SavableRepresentation data, Func<string, T> generateObj, string keyTypeName = "TypeName") where T : RepresentationObject
        {
            ensureLoaded();
            if (data == null)
            {
                string message = "No data found when attempting to load " + typeof(T).Name;
                logger.Log(typeof(NoDataFoundException).Name + ": " + message);
                throw new NoDataFoundException(message);
            }
            string typeName = data.GetValue(keyTypeName).GetString();
            T obj = generateObj(typeName);
            obj.RestoreFromRepresentation(data);
            return obj;
        }

        protected void SaveOne<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, T toSave, ref bool changed, string keyName = "UniqueID") where T : RepresentationObject, IKeyable
        {
            ensureLoaded();
            bool found = false;
            for (int i = 0; i < data.Count; i++)
            {
                var sr = data[i];
                if (sr.GetValue(keyName).GetUniqueID().Equals(toSave.UniqueID))
                {
                    data[i] = toSave.LoadIntoRepresentation();
                    found = true;
                    break;
                }
            }
            if (!found)
                data.Add(toSave.LoadIntoRepresentation());
            changed = true;
        }

        protected void SaveAll<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, IList<T> toSave, ref bool changed) where T : RepresentationObject, IKeyable
        {
            ensureLoaded();
            data = new List<SavableRepresentation>();
            foreach (var savable in toSave)
            {
                data.Add(savable.LoadIntoRepresentation());
            }
            changed = true;
        }

        protected void DeleteOne<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, T toDelete, ref bool changed, string keyName = "UniqueID") where T : RepresentationObject, IKeyable
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
            changed = true;
        }

        protected IList<T> LoadAll<T>(Action ensureLoaded, ref IList<SavableRepresentation> data, Func<string, T> generateObj, string keyTypeName = "TypeName") where T : RepresentationObject, IKeyable
        {
            return LoadAll<T, int>(ensureLoaded, ref data, generateObj, null, keyTypeName: keyTypeName);
        }

        protected IList<T> LoadAll<T, TSort>(Action ensureLoaded, ref IList<SavableRepresentation> data, Func<string, T> generateObj, Func<T, TSort> sortExpression, string keyTypeName = "TypeName") where T : RepresentationObject, IKeyable
        {
            ensureLoaded();
            if (data == null) data = new List<SavableRepresentation>();
            IList<T> list = new List<T>();
            foreach (var sr in data)
            {
                string typeName = sr.GetValue(keyTypeName).GetString().ToString();
                T obj;
                try
                {
                    obj = generateObj(typeName);
                }
                catch (ArgumentException e)
                {
                    logger.Log(GetType().Name + " LoadAll - error generating new object of type " + typeof(T).Name + " - " + e.Message);
                    throw;
                }
                obj.RestoreFromRepresentation(sr);
                list.Add(obj);
            }
            if (sortExpression != null) list = list.OrderBy(sortExpression).ToList();
            return list;
        }

        public virtual void Dispose()
        {
            if (!operationCanceled)
            {
                SaveModelObjectsIfLoaded();
                SaveSettingsIfLoaded();
                SaveScoreRangesIfLoaded();
                SaveStatusesIfLoaded();
                SaveCategoriesIfLoaded();
            }
            logger.Log(GetType().Name + " connection closed (canceled: " + operationCanceled.ToString() + ")");
        }
        #endregion

        #region "JSON Util"
        public static string FixJSONSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
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

        public static string CreateJSONArray(IEnumerable<RepresentationObject> objs)
        {
            ICollection<string> jsonObjs = new LinkedList<string>();
            foreach (RepresentationObject obj in objs)
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

        public static IList<SavableRepresentation> JSONToSavableRepresentationList(string json)
        {
            if (json == "")
            {
                return new List<SavableRepresentation>();
            }
            if (!IsValidJSONArray(json))
            {
                throw new JsonException("JSONToSavableRepresentationList: object is not valid json array: " + json);
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

        public static SavableRepresentation JSONToSavableRepresentation(string json)
        {
            if (json == "")
            {
                return new SavableRepresentation();
            }
            if (!IsValidJSONObject(json))
            {
                throw new JsonException("JSONToSavableRepresentation: object is not valid json object: " + json);
            }
            SavableRepresentation sr = new SavableRepresentation();
            JObject root = JObject.Parse(json);
            foreach (KeyValuePair<string, JToken> node in root)
            {
                if (node.Value is JArray array)
                {
                    // Node is an array
                    if (array.First is JValue)
                    {
                        // Array is all single values (assumes all values are same type)
                        LinkedList<string> stringList = new LinkedList<string>();
                        var objects = node.Value;
                        foreach (JValue childRoot in objects.Cast<JValue>())
                        {
                            stringList.AddLast(childRoot.Value.ToString());
                        }
                        sr.SaveValue(node.Key, new ValueContainer(stringList));
                    }
                    else if (array.First is JObject)
                    {
                        // Array is all json objects (assumes all values are same type)
                        LinkedList<SavableRepresentation> srList = new LinkedList<SavableRepresentation>();
                        var objects = node.Value;
                        foreach (JObject childRoot in objects.Cast<JObject>())
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

        public void SetCancel(bool cancel)
        {
            operationCanceled = cancel;
        }

        public void DeleteOneModelObject(RankedObject rankedObject)
        {
            DeleteOne(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObject, ref modelObjectsChanged);
        }

        public IList<RankedObject> LoadModelObjects(Settings settings, TrackerModule module)
        {
            return LoadAll(EnsureModelObjectsAreLoaded, ref modelObjects, (s) => factory.GetModelObject(s, settings, module), (obj) => obj.SortOrder);
        }

        public void SaveAllModelObjects(IList<RankedObject> rankedObjects)
        {
            SaveAll(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObjects, ref modelObjectsChanged);
        }

        public void SaveOneModelObject(RankedObject rankedObject)
        {
            SaveOne(EnsureModelObjectsAreLoaded, ref modelObjects, rankedObject, ref modelObjectsChanged);
        }

        public void SaveOneScoreRange(ScoreRange scoreRange)
        {
            SaveOne(EnsureScoreRangesAreLoaded, ref scoreRanges, scoreRange, ref scoreRangesChanged);
        }

        public void SaveAllScoreRanges(IList<ScoreRange> scoreRanges)
        {
            SaveAll(EnsureScoreRangesAreLoaded, ref this.scoreRanges, scoreRanges, ref scoreRangesChanged);
        }

        public void DeleteOneScoreRange(ScoreRange scoreRange)
        {
            DeleteOne(EnsureScoreRangesAreLoaded, ref scoreRanges, scoreRange, ref scoreRangesChanged);
        }

        public IList<ScoreRange> LoadScoreRanges(TrackerModuleScores module, SettingsScore settings)
        {
            return LoadAll(EnsureScoreRangesAreLoaded, ref scoreRanges, (s) => factory.GetScoreRange(s, module, settings));
        }

        public void SaveSettings(Settings settings)
        {
            SaveOne(EnsureSettingsAreLoaded, ref this.settings, settings, ref settingsChanged);
        }

        public Settings LoadSettings()
        {
            return LoadOne(EnsureSettingsAreLoaded, ref settings, (s) => factory.GetSettings(s));
        }

        public void SaveOneCategory(RatingCategory ratingCategory)
        {
            SaveOne(EnsureCategoriesAreLoaded, ref categories, ratingCategory, ref categoriesChanged);
        }

        public void SaveAllCategories(IList<RatingCategory> ratingCategories)
        {
            SaveAll(EnsureCategoriesAreLoaded, ref categories, ratingCategories, ref categoriesChanged);
        }

        public void DeleteOneCategory(RatingCategory ratingCategory)
        {
            DeleteOne(EnsureCategoriesAreLoaded, ref categories, ratingCategory, ref categoriesChanged);
        }

        public IList<RatingCategory> LoadCategories(IModuleCategorical module, SettingsScore settings)
        {
            return LoadAll(EnsureCategoriesAreLoaded, ref categories, (s) => factory.GetRatingCategory(s, module, settings));
        }

        public void SaveOneStatus(Status status)
        {
            SaveOne(EnsureStatusesAreLoaded, ref statuses, status, ref statusesChanged);
        }

        public void SaveAllStatuses(IList<Status> statuses)
        {
            SaveAll(EnsureStatusesAreLoaded, ref this.statuses, statuses, ref statusesChanged);
        }

        public void DeleteOneStatus(Status status)
        {
            DeleteOne(EnsureStatusesAreLoaded, ref statuses, status, ref statusesChanged);
        }

        public IList<Status> LoadStatuses(IModuleStatus module, Settings settings)
        {
            return LoadAll(EnsureStatusesAreLoaded, ref statuses, (s) => factory.GetStatus(s, module, settings));
        }
    }
}
