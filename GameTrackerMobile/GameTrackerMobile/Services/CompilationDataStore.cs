using GameTracker.Model;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMobile.Services
{
    public class CompilationDataStore : BaseDataStore<GameCompilation>
    {
        public CompilationDataStore() : base() { }

        public override async Task<bool> AddItemAsync(GameCompilation item)
        {
            RefreshModule();
            rm.AddGameCompilation(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(GameCompilation item)
        {
            RefreshModule();
            var oldItem = rm.FindGameCompilation(new ObjectReference(item));
            rm.UpdateGameCompilation(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(GameCompilation item)
        {
            RefreshModule();
            rm.DeleteGameCompilation(item);

            return await Task.FromResult(true);
        }

        public override async Task<GameCompilation> GetItemAsync(ObjectReference key)
        {
            RefreshModule();
            return await Task.FromResult(rm.FindGameCompilation(key));
        }

        public override async Task<IEnumerable<GameCompilation>> GetItemsAsync(bool forceRefresh = false)
        {
            RefreshModule();
            return await Task.FromResult(rm.GameCompilations);
        }
    }
}
