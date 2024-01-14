using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
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

namespace RatableTracker.Modules
{
    public class TrackerModule : ModuleBase
    {
        public virtual int LimitModelObjects => 100000;

        private IList<RankedObject> _modelObjects = new List<RankedObject>();
        protected IList<RankedObject> ModelObjects { get { return _modelObjects; } private set { _modelObjects = value; } }

        public delegate void ModelObjectDeleteHandler(object sender, ModelObjectDeleteArgs args);
        public event ModelObjectDeleteHandler ModelObjectDeleted;

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave) : base(loadSave) { }

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger) : base(loadSave, logger) { }

        protected override void LoadDataConsecutively(Settings settings, ILoadSaveMethod conn)
        {
            LoadTrackerObjectList(ref _modelObjects, conn, (conn) => conn.LoadModelObjects(settings, this));
        }

        protected override IList<Task> LoadDataCreateTaskList(Settings settings, ILoadSaveMethod conn)
        {
            return new List<Task>
            {
                Task.Run(() => LoadTrackerObjectList(ref _modelObjects, conn, (conn) => conn.LoadModelObjects(settings, this)))
            };
        }

        public void TransferToNewModule(TrackerModule newModule, Settings settings)
        {
            using var connCurrent = LoadSave.NewConnection();
            using var connNew = newModule.LoadSave.NewConnection();
            TransferToNewModule(connCurrent, connNew, settings);
        }

        protected void TransferToNewModule(ILoadSaveMethod connCurrent, ILoadSaveMethod connNew, Settings settings)
        {
            connNew.SaveAllModelObjects(connCurrent.LoadModelObjects(settings, this));
            Settings settingsTransfer;
            try
            {
                settingsTransfer = connCurrent.LoadSettings();
            }
            catch (NoDataFoundException)
            {
                settingsTransfer = null;
            }
            connNew.SaveSettings(settingsTransfer);
        }

        public IList<RankedObject> GetModelObjectList(Settings settings)
        {
            return GetModelObjectList<RankedObject>(null, null, settings);
        }

        public IList<T> GetModelObjectList<T>(FilterEngine filterEngine, Settings settings) where T : RankedObject
        {
            return GetModelObjectList<T>(filterEngine, null, settings);
        }

        public IList<T> GetModelObjectList<T>(SortEngine sortEngine, Settings settings) where T : RankedObject
        {
            return GetModelObjectList<T>(null, sortEngine, settings);
        }

        public IList<T> GetModelObjectList<T>(FilterEngine filterEngine, SortEngine sortEngine, Settings settings) where T : RankedObject
        {
            return GetTrackerObjectList(ModelObjects.OfType<T>().ToList(), filterEngine, sortEngine, (conn) => conn.LoadModelObjectsAndFilter<T>(settings, this, filterEngine, sortEngine));
        }

        public int TotalNumModelObjects()
        {
            return ModelObjects.Count;
        }

        internal bool SaveModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            return SaveTrackerObject(modelObject, ref _modelObjects, LimitModelObjects, conn.SaveOneModelObject);
        }

        internal void DeleteModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            DeleteTrackerObject(modelObject, ref _modelObjects, conn.DeleteOneModelObject,
                (obj) => ModelObjectDeleted?.Invoke(this, new ModelObjectDeleteArgs(obj, obj.GetType(), conn)), () => ModelObjectDeleted == null ? 0 : ModelObjectDeleted.GetInvocationList().Length);
        }

        internal void ChangeModelObjectPositionInList(RankedObject modelObject, int newPosition, Settings settings)
        {
            ChangeTrackerObjectPositionInList(modelObject, ref _modelObjects, newPosition, this, settings);
        }
    }
}
