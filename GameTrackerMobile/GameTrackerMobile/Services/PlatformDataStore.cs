using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework;

namespace GameTrackerMobile.Services
{
    public class PlatformDataStore : BaseDataStore<Platform>
    {
        public PlatformDataStore() : base() { }

        public override async Task<bool> AddItemAsync(Platform item)
        {
            rm.AddPlatform(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(Platform item)
        {
            var oldItem = rm.FindPlatform(new ObjectReference(item));
            rm.UpdatePlatform(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(Platform item)
        {
            rm.DeletePlatform(item);

            return await Task.FromResult(true);
        }

        public override async Task<Platform> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindPlatform(key));
        }

        public override async Task<IEnumerable<Platform>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.Platforms);
        }
    }
}
