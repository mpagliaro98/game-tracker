using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework;

namespace GameTrackerMobile.Services
{
    public interface IDataStore<T> where T : IReferable
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(T item);
        Task<T> GetItemAsync(ObjectReference key);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
