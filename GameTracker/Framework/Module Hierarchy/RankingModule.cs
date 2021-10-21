using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.ScoreRelationships;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RankingModule<TListedObj, TRange, TSettings>
        where TListedObj : ListedObject
        where TRange : ScoreRange
        where TSettings : Settings
    {
        protected IEnumerable<TListedObj> listedObjs;
        protected IEnumerable<TRange> ranges;
        protected IEnumerable<ScoreRelationship> scoreRelationships;
        protected TSettings settings;

        public IEnumerable<TListedObj> ListedObjects
        {
            get { return listedObjs; }
        }

        public IEnumerable<TRange> Ranges
        {
            get { return ranges; }
        }

        public TSettings Settings
        {
            get { return settings; }
        }

        public IEnumerable<ScoreRelationship> ScoreRelationships
        {
            get { return scoreRelationships; }
        }

        public virtual int LimitRanges => 20;

        public RankingModule()
        {
            CreateScoreRelationshipsList();
        }

        protected virtual void CreateScoreRelationshipsList()
        {
            scoreRelationships = new List<ScoreRelationship>();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipBetween()).ToList();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipAbove()).ToList();
            scoreRelationships = scoreRelationships.Append(new ScoreRelationshipBelow()).ToList();
        }

        public virtual void Init()
        {
            LoadSettings();
            LoadRanges();
            LoadListedObjects();
        }

        public abstract void LoadListedObjects();
        public abstract void LoadRanges();
        public abstract void LoadSettings();
        public abstract void SaveListedObjects();
        public abstract void SaveRanges();
        public abstract void SaveSettings();

        protected T FindObject<T>(IEnumerable<T> sourceList, ObjectReference objectKey) where T : IReferable
        {
            if (!objectKey.HasReference()) return default;
            foreach (IReferable obj in sourceList)
            {
                if (objectKey.IsReferencedObject(obj))
                {
                    return (T)obj;
                }
            }
            throw new ReferenceNotFoundException(GetType().Name + ": could not find object in " + sourceList.ToString() + " with key " + objectKey.ObjectKey.ToString());
        }

        public TListedObj FindListedObject(ObjectReference objectKey)
        {
            return FindObject(listedObjs, objectKey);
        }

        public TRange FindRange(ObjectReference objectKey)
        {
            return FindObject(ranges, objectKey);
        }

        public ScoreRelationship FindScoreRelationship(ObjectReference objectKey)
        {
            return FindObject(scoreRelationships, objectKey);
        }

        public virtual System.Drawing.Color GetColorFromRange(TListedObj obj)
        {
            int i = 1;
            foreach (TListedObj objLoop in ListedObjects)
            {
                if (objLoop.Equals(obj))
                {
                    TRange range = ApplyRange(i);
                    return range == null ? new System.Drawing.Color() : range.Color;
                }
                i++;
            }
            return new System.Drawing.Color();
        }

        protected TRange ApplyRange(double val)
        {
            foreach (TRange sr in Ranges)
            {
                ScoreRelationship relationship = FindScoreRelationship(sr.RefScoreRelationship);
                if (relationship.IsValueInRange(val, sr.ValueList))
                {
                    return sr;
                }
            }
            return null;
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            AddToList(ref list, saveFunction, obj, -1);
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int limit)
        {
            if (limit >= 0 && list.Count() >= limit)
            {
                throw new ExceededLimitException("Attempted to exceed limit of " + limit.ToString() + " for list of " + typeof(T).ToString());
            }
            list = list.Append(obj);
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, T orig)
        {
            List<T> temp = list.ToList();
            int idx = temp.IndexOf(orig);
            UpdateInList(ref list, saveFunction, obj, idx);
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int idx)
        {
            List<T> temp = list.ToList();
            temp[idx] = obj;
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void DeleteFromList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            List<T> temp = list.ToList();
            temp.Remove(obj);
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        public void AddListedObject(TListedObj obj)
        {
            AddToList(ref listedObjs, SaveListedObjects, obj);
        }

        public void AddRange(TRange obj)
        {
            AddToList(ref ranges, SaveRanges, obj, LimitRanges);
        }

        public void UpdateListedObject(TListedObj obj, TListedObj orig)
        {
            UpdateInList(ref listedObjs, SaveListedObjects, obj, orig);
        }

        public void UpdateRange(TRange obj, TRange orig)
        {
            UpdateInList(ref ranges, SaveRanges, obj, orig);
        }

        public void DeleteListedObject(TListedObj obj)
        {
            DeleteFromList(ref listedObjs, SaveListedObjects, obj);
        }

        public void DeleteRange(TRange obj)
        {
            DeleteFromList(ref ranges, SaveRanges, obj);
        }
    }
}
