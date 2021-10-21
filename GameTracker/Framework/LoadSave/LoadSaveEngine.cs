using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.ModuleHierarchy;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngine<TListedObj, TRange, TSettings>
        where TListedObj : ListedObject, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : Settings, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_LISTEDOBJECTS = new LoadSaveIdentifier("ListedObjects");
        protected static LoadSaveIdentifier ID_RANGES = new LoadSaveIdentifier("Ranges");
        protected static LoadSaveIdentifier ID_SETTINGS = new LoadSaveIdentifier("Settings");

        protected abstract IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract T LoadISavable<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable;
        protected abstract void SaveISavable<T>(T obj, LoadSaveIdentifier id) where T : ISavable;

        public virtual IEnumerable<TListedObj> LoadListedObjects()
        {
            return LoadISavableList<TListedObj>(ID_LISTEDOBJECTS);
        }

        public virtual IEnumerable<TRange> LoadRanges()
        {
            return LoadISavableList<TRange>(ID_RANGES);
        }

        public virtual TSettings LoadSettings()
        {
            return LoadISavable<TSettings>(ID_SETTINGS);
        }

        public virtual void SaveListedObjects(IEnumerable<TListedObj> ratableObjects)
        {
            SaveISavableList(ratableObjects, ID_LISTEDOBJECTS);
        }

        public virtual void SaveRanges(IEnumerable<TRange> scoreRanges)
        {
            SaveISavableList(scoreRanges, ID_RANGES);
        }

        public virtual void SaveSettings(TSettings settings)
        {
            SaveISavable(settings, ID_SETTINGS);
        }
    }
}
