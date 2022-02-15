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
            RefreshModule();
            rm.AddPlatform(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(Platform item)
        {
            RefreshModule();
            var oldItem = rm.FindPlatform(new ObjectReference(item));
            rm.UpdatePlatform(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(Platform item)
        {
            RefreshModule();
            rm.DeletePlatform(item);

            return await Task.FromResult(true);
        }

        public override async Task<Platform> GetItemAsync(ObjectReference key)
        {
            RefreshModule();
            return await Task.FromResult(rm.FindPlatform(key));
        }

        public override async Task<IEnumerable<Platform>> GetItemsAsync(bool forceRefresh = false)
        {
            RefreshModule();
            return await Task.FromResult(rm.Platforms);
        }
    }
}
