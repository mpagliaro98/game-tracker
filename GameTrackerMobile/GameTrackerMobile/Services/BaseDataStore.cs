using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework;

namespace GameTrackerMobile.Services
{
    public abstract class BaseDataStore<T> : IDataStore<T> where T : IReferable
    {
        protected readonly RatingModuleGame rm;

        public BaseDataStore()
        {
            rm = ModuleService.GetActiveModule();
        }

        public abstract Task<bool> AddItemAsync(T item);

        public abstract Task<bool> DeleteItemAsync(T item);

        public abstract Task<T> GetItemAsync(ObjectReference key);

        public abstract Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        public abstract Task<bool> UpdateItemAsync(T item);
    }
}
