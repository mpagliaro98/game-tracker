﻿using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
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
        public ILogger Logger { get; private set; }

        public TrackerModule(ILoadSaveHandler<ILoadSaveMethod> loadSave, ILogger logger = null)
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

        internal void SaveModelObject(RankedObject modelObject, ILoadSaveMethod conn)
        {
            Logger?.Log("SaveModelObject - " + modelObject.UniqueID.ToString());

            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                if (ModelObjects.Count() >= LimitModelObjects)
                {
                    try
                    {
                        throw new ExceededLimitException("Attempted to exceed limit of " + LimitModelObjects.ToString() + " for list of model objects");
                    }
                    catch (ExceededLimitException e)
                    {
                        Logger?.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
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
            Logger?.Log("DeleteModelObject - " + modelObject.UniqueID.ToString());
            if (Util.Util.FindObjectInList(ModelObjects, modelObject.UniqueID) == null)
            {
                try
                {
                    throw new InvalidObjectStateException("Model object " + modelObject.Name.ToString() + " has not been saved yet and cannot be deleted");
                }
                catch (InvalidObjectStateException e)
                {
                    Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
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
            Logger?.Log("ChangeModelObjectPositionInList - " + modelObject.UniqueID.ToString() + " - " + newPosition.ToString());
            modelObject.Validate(Logger);

            int currentPosition = ModelObjects.IndexOf(modelObject);
            if (currentPosition == -1)
            {
                try
                {
                    throw new InvalidObjectStateException("Object " + modelObject.UniqueID.ToString() + " has not been saved yet and cannot be modified");
                }
                catch (InvalidObjectStateException e)
                {
                    Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
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
