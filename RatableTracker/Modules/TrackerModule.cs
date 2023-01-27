using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RatableTracker.Modules
{
    public class TrackerModule
    {
        public virtual int LimitModelObjects => 100000;

        protected IList<RankedObject> ModelObjects { get; private set; } = new List<RankedObject>();

        public delegate void ModelObjectDeleteHandler(object sender, ModelObjectDeleteArgs args);
        public event ModelObjectDeleteHandler ModelObjectDeleted;

        protected readonly ILoadSaveHandler<ILoadSaveMethod> _loadSave;
        public Logger Logger { get; private set; }

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger)
        {
            _loadSave = loadSave;
            Logger = logger;
        }

        public virtual void LoadData(Settings settings)
        {
            using (var conn = _loadSave.NewConnection())
            {
                ModelObjects = conn.LoadModelObjects(settings, this);
            }
        }

        public void TransferToNewModule(TrackerModule newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected void TransferToNewModule(ILoadSaveMethod connCurrent, ILoadSaveMethod connNew, Settings settings)
        {
            connNew.SaveAllModelObjects(connCurrent.LoadModelObjects(settings, this));
            connNew.SaveSettings(connCurrent.LoadSettings());
        }

        public IList<RankedObject> GetModelObjectList()
        {
            return GetModelObjectList(null, null);
        }

        public IList<RankedObject> GetModelObjectList(FilterRankedObjects filterOptions)
        {
            return GetModelObjectList(filterOptions, null);
        }

        public IList<RankedObject> GetModelObjectList(SortRankedObjects sortOptions)
        {
            return GetModelObjectList(null, sortOptions);
        }

        public IList<RankedObject> GetModelObjectList(FilterRankedObjects filterOptions, SortRankedObjects sortOptions)
        {
            try
            {
                IList<RankedObject> list = new List<RankedObject>(ModelObjects);
                if (filterOptions != null) list = filterOptions.ApplyFilters(list);
                if (sortOptions != null) list = sortOptions.ApplySorting(list);
                return list;
            }
            catch (ListManipulationException e)
            {
                Logger.Log(e.GetType().Name + ": " + e.Message + " - value " + e.InvalidValue.ToString());
                throw;
            }
        }

        public int TotalNumModelObjects()
        {
            return ModelObjects.Count;
        }

        internal bool SaveModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            Logger.Log("SaveModelObject - " + modelObject.UniqueID.ToString());
            bool isNew = false;
            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                if (ModelObjects.Count() >= LimitModelObjects)
                {
                    string message = "Attempted to exceed limit of " + LimitModelObjects.ToString() + " for list of model objects";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                ModelObjects.Add(modelObject);
                isNew = true;
            }
            else
            {
                var old = ModelObjects.Replace(modelObject);
                if (old != modelObject)
                    old.Dispose();
            }
            conn.SaveOneModelObject(modelObject);
            return isNew;
        }

        internal void DeleteModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            Logger.Log("DeleteModelObject - " + modelObject.UniqueID.ToString());
            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                string message = "Model object " + modelObject.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            ModelObjects.Remove(modelObject);
            conn.DeleteOneModelObject(modelObject);
            ModelObjectDeleted?.Invoke(this, new ModelObjectDeleteArgs(modelObject, modelObject.GetType(), conn));
        }

        internal void ChangeModelObjectPositionInList(RankedObject modelObject, int newPosition)
        {
            Logger.Log("ChangeModelObjectPositionInList - " + modelObject.UniqueID.ToString() + " - " + newPosition.ToString());
            modelObject.Validate(Logger);

            int currentPosition = ModelObjects.IndexOf(modelObject);
            if (currentPosition == -1)
            {
                string message = "Object " + modelObject.UniqueID.ToString() + " has not been saved yet and cannot be modified";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }

            ModelObjects.Move(currentPosition, newPosition);

            using (var conn = _loadSave.NewConnection())
            {
                for (int i = currentPosition; i <= newPosition; i++)
                {
                    ModelObjects[i].Save(this, conn);
                }
            }
        }

        internal void SaveSettings(Settings settings, ILoadSaveMethod conn)
        {
            Logger.Log("SaveSettings");
            conn.SaveSettings(settings);
        }

        public virtual void ApplySettingsChanges(Settings settings, ILoadSaveMethod conn)
        {
            foreach (RankedObject obj in ModelObjects)
            {
                obj.ApplySettingsChanges(settings);
                obj.Save(this, conn);
            }
        }

        internal ILoadSaveMethod GetNewConnection()
        {
            return _loadSave.NewConnection();
        }
    }
}
