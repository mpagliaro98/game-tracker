using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModule
    {
        public virtual int LimitRankedObjects => 100000;

        private IList<RankedObject> _modelObjects = new List<RankedObject>();
        protected IList<RankedObject> ModelObjects => _modelObjects;

        protected readonly ILoadSaveHandler<ILoadSaveMethod> _loadSave;

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave)
        {
            _loadSave = loadSave;
        }

        public virtual void Init()
        {
            using (var conn = _loadSave.NewConnection())
            {
                _modelObjects = conn.LoadModelObjects();
            }
        }

        public void TransferToNewModule(TrackerModule newModule)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew);
                }
            }
        }

        protected void TransferToNewModule(ILoadSaveMethod connCurrent, ILoadSaveMethod connNew)
        {
            connNew.SaveAllModelObjects(connCurrent.LoadModelObjects());
        }

        /// <summary>
        /// Get the list of listed objects, with any filtering and sorting applied.
        /// </summary>
        public IList<RankedObject> GetModelObjectList()
        {
            // TODO pass in filter and sort options with function overloads
            return new List<RankedObject>(ModelObjects);
        }

        public int TotalNumModelObjects()
        {
            return ModelObjects.Count;
        }

        public void SaveModelObject(RankedObject modelObject)
        {
            // TODO throw unique exception
            modelObject.Validate();
            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                if (ModelObjects.Count() >= LimitRankedObjects)
                    throw new Exception("Attempted to exceed limit of " + LimitRankedObjects.ToString() + " for list of model objects");
                ModelObjects.Add(modelObject);
            }
            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneModelObject(modelObject);
            }
        }

        public void DeleteModelObject(RankedObject modelObject)
        {
            // TODO throw unique exception
            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
                throw new Exception("Model object " + modelObject.Name.ToString() + " has not been saved yet and cannot be deleted");
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneModelObject(modelObject);
            }
        }

        public void ChangeModelObjectPositionInList(RankedObject modelObject, int newPosition)
        {
            // TODO throw unique exception
            modelObject.Validate();

            int currentPosition = ModelObjects.IndexOf(modelObject);
            if (currentPosition == -1)
                throw new Exception("Object " + modelObject.UniqueID.ToString() + " does not exist in the module");

            ModelObjects.RemoveAt(currentPosition);
            ModelObjects.Insert(newPosition, modelObject);

            using (var conn = _loadSave.NewConnection())
            {
                for (int i = currentPosition; i <= newPosition; i++)
                {
                    conn.SaveOneModelObject(ModelObjects[i]);
                }
            }
        }
    }
}
