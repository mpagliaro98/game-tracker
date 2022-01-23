using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.IO;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework;

namespace GameTrackerMobile.Services
{
    public abstract class BaseDataStore<T> : IDataStore<T> where T : IReferable
    {
        protected readonly RatingModuleGame rm;

        public BaseDataStore()
        {
            PathController.PathControllerInstance = new PathControllerMobile();
            GlobalSettings.Autosave = true;
            IContentLoadSave<string, string> cls;
            //if (ContentLoadSaveAWSS3.KeyFileExists())
            //    cls = new ContentLoadSaveAWSS3();
            //else
            cls = new ContentLoadSaveLocal();
            LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
            {
                ContentLoadSaveInstance = cls
            };
            rm = new RatingModuleGame(engine);
            rm.Init();
        }

        public abstract Task<bool> AddItemAsync(T item);

        public abstract Task<bool> DeleteItemAsync(T item);

        public abstract Task<T> GetItemAsync(ObjectReference key);

        public abstract Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        public abstract Task<bool> UpdateItemAsync(T item);
    }
}
