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
        public LoadSaveIdentifier ID_LISTEDOBJECTS => new LoadSaveIdentifier("ListedObjects");
        public LoadSaveIdentifier ID_RANGES => new LoadSaveIdentifier("Ranges");
        public LoadSaveIdentifier ID_SETTINGS => new LoadSaveIdentifier("Settings");

        public abstract IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id) where T : ISavable, new();
        public abstract Task<IEnumerable<T>> LoadISavableListAsync<T>(LoadSaveIdentifier id) where T : ISavable, new();
        public abstract T LoadISavable<T>(LoadSaveIdentifier id) where T : ISavable, new();
        public abstract Task<T> LoadISavableAsync<T>(LoadSaveIdentifier id) where T : ISavable, new();
        public abstract void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable;
        public abstract Task SaveISavableListAsync<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable;
        public abstract void SaveISavable<T>(T obj, LoadSaveIdentifier id) where T : ISavable;
        public abstract Task SaveISavableAsync<T>(T obj, LoadSaveIdentifier id) where T : ISavable;
        public abstract void TransferSaveFiles(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to);
        public abstract Task TransferSaveFilesAsync(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to);
    }
}
