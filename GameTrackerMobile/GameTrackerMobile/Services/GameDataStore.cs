using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework;

namespace GameTrackerMobile.Services
{
    public class GameDataStore : BaseDataStore<RatableGame>
    {
        public GameDataStore() : base() { }

        public override async Task<bool> AddItemAsync(RatableGame item)
        {
            rm.AddListedObject(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(RatableGame item)
        {
            var oldItem = rm.FindListedObject(new ObjectReference(item));
            rm.UpdateListedObject(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(RatableGame item)
        {
            rm.DeleteListedObject(item);

            return await Task.FromResult(true);
        }

        public override async Task<RatableGame> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindListedObject(key));
        }

        public override async Task<IEnumerable<RatableGame>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.ListedObjects);
        }
    }
}