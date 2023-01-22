using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.ListManipulation;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModule
    {
        public virtual int LimitModelObjects => 100000;

        protected IList<RankedObject> ModelObjects { get; private set; } = new List<RankedObject>();

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

        public virtual void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RankedObject rankedObject in ModelObjects)
                {
                    if (rankedObject.RemoveReferenceToObject(obj, type))
                    {
                        rankedObject.Save(this, conn);
                    }
                }
            }
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

        internal void SaveModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            Logger.Log("SaveModelObject - " + modelObject.UniqueID.ToString());

            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                if (ModelObjects.Count() >= LimitModelObjects)
                {
                    string message = "Attempted to exceed limit of " + LimitModelObjects.ToString() + " for list of model objects";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                ModelObjects.Add(modelObject);
            }

            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.SaveOneModelObject(modelObject);
                }
            }
            else
            {
                conn.SaveOneModelObject(modelObject);
            }
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
            RemoveReferencesToObject(modelObject, typeof(RankedObject));
            ModelObjects.Remove(modelObject);
            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.DeleteOneModelObject(modelObject);
                }
            }
            else
            {
                conn.DeleteOneModelObject(modelObject);
            }
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
            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.SaveSettings(settings);
                }
            }
            else
            {
                conn.SaveSettings(settings);
            }
        }

        public virtual void ApplySettingsChanges(Settings settings)
        {
            foreach (RankedObject obj in ModelObjects)
            {
                obj.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (RankedObject obj in ModelObjects)
                {
                    obj.Save(this, conn);
                }
            }
        }
    }
}
