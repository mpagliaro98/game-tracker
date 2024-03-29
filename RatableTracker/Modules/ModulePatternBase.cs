﻿using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public abstract class ModulePatternBase
    {
        protected ILoadSaveHandler<ILoadSaveMethod> LoadSave { get; init; }
        public Logger Logger { get; init; }

        public ModulePatternBase(ILoadSaveHandler<ILoadSaveMethod> loadSave) : this(loadSave, new Logger()) { }

        public ModulePatternBase(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger)
        {
            LoadSave = loadSave;
            Logger = logger;
        }

        protected void LoadTrackerObjectList<T>(ref IList<T> list, ILoadSaveMethod conn, Func<ILoadSaveMethod, IList<T>> loadFunc) where T : TrackerObjectBase
        {
            list.ForEach(obj => obj.Dispose());
            try
            {
                list = loadFunc(conn);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("A value passed in during load must be a more derived type", e);
            }
            list.ForEach(obj => obj.InitAdditionalResources());
        }

        protected bool SaveTrackerObject<T>(T obj, ref IList<T> list, int limit, Action<T> saveOneFunc) where T : TrackerObjectBase
        {
            Logger.Log("Save " + obj.GetType().Name + " - " + obj.UniqueID.ToString() + " " + obj.Name);
            bool isNew = false;
            if (Util.Util.FindObjectInList(list, obj.UniqueID) == null)
            {
                if (list.Count >= limit)
                {
                    string message = "Attempted to exceed limit of " + limit.ToString() + " for list of " + obj.GetType().Name;
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                list.Add(obj);
                obj.InitAdditionalResources();
                isNew = true;
            }
            else
            {
                var old = list.Replace(obj);
                if (old != obj)
                {
                    old.Dispose();
                    obj.InitAdditionalResources();
                }
            }
            saveOneFunc(obj);
            return isNew;
        }

        protected void DeleteTrackerObject<T>(T obj, ref IList<T> list, Action<T> deleteOneFunc, Action<T> invokeDeleteEvent, Func<int> numInvokeTargets) where T : TrackerObjectBase
        {
            Logger.Log("Delete " + obj.GetType().Name + " - " + obj.UniqueID.ToString() + " " + obj.Name);
            if (Util.Util.FindObjectInList(list, obj.UniqueID) == null)
            {
                string message = obj.GetType().Name + " " + obj.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            list.Remove(obj);
            obj.Dispose();
            deleteOneFunc(obj);
            Logger.Log(obj.GetType().Name + " deleted - invoking delete event on " + numInvokeTargets().ToString() + " delegates");
            invokeDeleteEvent(obj);
        }

        protected IList<T> GetTrackerObjectList<T>(IList<T> originalList) where T : TrackerObjectBase
        {
            return GetTrackerObjectList(originalList, null, null, false, null);
        }

        protected IList<T> GetTrackerObjectList<T>(IList<T> originalList, FilterEngine filterEngine, SortEngine sortEngine, Func<ILoadSaveMethod, IList<T>> loadAndFilter) where T : TrackerObjectBase
        {
            return GetTrackerObjectList(originalList, filterEngine, sortEngine, true, loadAndFilter);
        }

        private IList<T> GetTrackerObjectList<T>(IList<T> originalList, FilterEngine filterEngine, SortEngine sortEngine, bool supportsLoadAndFilter, Func<ILoadSaveMethod, IList<T>> loadAndFilter) where T : TrackerObjectBase
        {
            try
            {
                if (supportsLoadAndFilter && LoadSave.FilterFromLoadSave)
                {
                    using var conn = LoadSave.NewConnection();
                    return loadAndFilter(conn);
                }
                else
                {
                    IList<T> list = new List<T>(originalList);
                    if (filterEngine != null) list = filterEngine.ApplyFilters(list);
                    if (sortEngine != null) list = sortEngine.ApplySorting(list);
                    return list;
                }
            }
            catch (ListManipulationException e)
            {
                Logger.Log(e.GetType().Name + ": " + e.Message + " - value " + (e.InvalidValue?.ToString() ?? "(null)"));
                throw;
            }
        }

        protected void ChangeTrackerObjectPositionInList<T>(T obj, ref IList<T> list, int newPosition, TrackerModule module, Settings settings) where T : TrackerObjectBase
        {
            Logger.Log("ChangeTrackerObjectPositionInList " + obj.GetType().Name + " - " + obj.UniqueID.ToString() + " " + obj.Name + " - " + newPosition.ToString());
            obj.Validate(Logger);

            int currentPosition = list.IndexOf(obj);
            if (currentPosition == -1)
            {
                string message = obj.GetType().Name + " " + obj.UniqueID.ToString() + " has not been saved yet and cannot be modified";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }

            list.Move(currentPosition, newPosition);

            using var conn = LoadSave.NewConnection();
            for (int i = currentPosition; i <= newPosition; i++)
            {
                list[i].SaveWithoutValidation(module, settings, conn);
            }
        }
    }
}
