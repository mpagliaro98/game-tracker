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

        public IEnumerable<TListedObj> ListedObjects => listedObjs;

        public IEnumerable<TRange> Ranges => ranges;

        public TSettings Settings => settings;

        public IEnumerable<ScoreRelationship> ScoreRelationships => scoreRelationships;

        public virtual int LimitRanges => 20;

        public virtual int LimitListedObjects => 100000;

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

        public virtual System.Drawing.Color GetRangeColorFromObject(TListedObj obj)
        {
            int rank = GetRankOfObject(obj);
            return GetRangeColorFromValue(rank);
        }

        public virtual System.Drawing.Color GetRangeColorFromValue(double val)
        {
            TRange range = ApplyRange(val);
            return range == null ? new System.Drawing.Color() : range.Color;
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

        public virtual int GetRankOfObject(TListedObj obj)
        {
            int i = 1;
            foreach (TListedObj objLoop in ListedObjects)
            {
                if (objLoop.Equals(obj))
                {
                    return i;
                }
                i++;
            }
            return i;
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            AddToList(ref list, saveFunction, obj, -1);
        }

        protected void AddToList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int limit)
        {
            if (limit >= 0 && list.Count() >= limit)
                throw new ExceededLimitException("Attempted to exceed limit of " + limit.ToString() + " for list of " + typeof(T).ToString());
            list = list.Append(obj);
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void InsertIntoList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int position)
        {
            InsertIntoList(ref list, saveFunction, obj, position, -1);
        }

        protected void InsertIntoList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int position, int limit)
        {
            if (limit >= 0 && list.Count() >= limit)
                throw new ExceededLimitException("Attempted to exceed limit of " + limit.ToString() + " for list of " + typeof(T).ToString());
            List<T> temp = list.ToList();
            temp.Insert(position, obj);
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, T orig)
        {
            if (obj is IReferable objReferable && orig is IReferable origReferable)
                objReferable.OverwriteReferenceKey(origReferable);
            List<T> temp = list.ToList();
            int idx = temp.IndexOf(orig);
            UpdateInList(ref list, saveFunction, obj, idx);
        }

        protected void UpdateInList<T>(ref IEnumerable<T> list, Action saveFunction, T obj, int idx)
        {
            if (idx < 0 || idx >= list.Count()) throw new ArgumentOutOfRangeException();
            List<T> temp = list.ToList();
            temp[idx] = obj;
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected void DeleteFromList<T>(ref IEnumerable<T> list, Action saveFunction, T obj)
        {
            List<T> temp = list.ToList();
            if (!temp.Remove(obj)) throw new ArgumentOutOfRangeException();
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        protected IEnumerable<TInput> SortList<TInput, TOutput>(IEnumerable<TInput> list,
            Func<TInput, TOutput> keySelector, SortMode mode)
        {
            if (mode == SortMode.ASCENDING)
                return list.OrderBy(keySelector);
            else if (mode == SortMode.DESCENDING)
                return list.OrderByDescending(keySelector);
            else
                throw new Exception("Unhandled sort mode");
        }

        protected void MoveElementInList<T>(ref IEnumerable<T> list, Action saveFunction, int posToChange, int newPos)
        {
            List<T> temp = list.ToList();
            T element = temp.ElementAt(posToChange);
            MoveElementInList(ref list, saveFunction, element, newPos);
        }

        protected void MoveElementInList<T>(ref IEnumerable<T> list, Action saveFunction, T element, int newPos)
        {
            List<T> temp = list.ToList();
            if (!temp.Remove(element)) throw new ArgumentOutOfRangeException();
            temp.Insert(newPos, element);
            list = temp;
            if (GlobalSettings.Autosave) saveFunction();
        }

        public void AddListedObject(TListedObj obj)
        {
            ValidateListedObject(obj);
            AddToList(ref listedObjs, SaveListedObjects, obj, LimitListedObjects);
        }

        public void AddListedObjectAtPosition(TListedObj obj, int position)
        {
            ValidateListedObject(obj);
            InsertIntoList(ref listedObjs, SaveListedObjects, obj, position, LimitListedObjects);
        }

        public void AddRange(TRange obj)
        {
            ValidateRange(obj);
            AddToList(ref ranges, SaveRanges, obj, LimitRanges);
        }

        public void UpdateListedObject(TListedObj obj, TListedObj orig)
        {
            ValidateListedObject(obj);
            UpdateInList(ref listedObjs, SaveListedObjects, obj, orig);
        }

        public void UpdateRange(TRange obj, TRange orig)
        {
            ValidateRange(obj);
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

        public IEnumerable<TListedObj> SortListedObjects<TField>(Func<TListedObj, TField> keySelector, SortMode mode = SortMode.ASCENDING)
        {
            return SortList(listedObjs, keySelector, mode);
        }

        public IEnumerable<TRange> SortRanges<TField>(Func<TRange, TField> keySelector, SortMode mode = SortMode.ASCENDING)
        {
            return SortList(ranges, keySelector, mode);
        }

        public void ChangeRankOfListedObject(int rankToChange, int newRank)
        {
            MoveElementInList(ref listedObjs, SaveListedObjects, rankToChange, newRank);
        }

        public void ChangeRankOfListedObject(TListedObj obj, int newRank)
        {
            MoveElementInList(ref listedObjs, SaveListedObjects, obj, newRank);
        }

        public void MoveObjectUpOneRank(TListedObj obj)
        {
            int position = listedObjs.ToList().IndexOf(obj);
            MoveObjectUpOneRank(position);
        }

        public void MoveObjectUpOneRank(int rank)
        {
            MoveElementInList(ref listedObjs, SaveListedObjects, rank, rank - 1);
        }

        public void MoveObjectDownOneRank(TListedObj obj)
        {
            int position = listedObjs.ToList().IndexOf(obj);
            MoveObjectDownOneRank(position);
        }

        public void MoveObjectDownOneRank(int rank)
        {
            MoveElementInList(ref listedObjs, SaveListedObjects, rank, rank - 1);
        }

        public virtual void ValidateListedObject(TListedObj obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > ListedObject.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + ListedObject.MaxLengthName.ToString() + " characters");
            if (obj.Comment.Length > ListedObject.MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + ListedObject.MaxLengthComment.ToString() + " characters");
        }

        public virtual void ValidateRange(TRange obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > ScoreRange.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + ScoreRange.MaxLengthName.ToString() + " characters");
            if (!obj.RefScoreRelationship.HasReference())
                throw new ValidationException("A score relationship is required");
            ScoreRelationship sr = FindScoreRelationship(obj.RefScoreRelationship);
            if (sr.NumValuesRequired != obj.ValueList.Count())
                throw new ValidationException("The " + sr.Name + " relationship requires " + sr.NumValuesRequired.ToString() + " values, but " + obj.ValueList.Count().ToString() + " were given");
        }
    }
}
