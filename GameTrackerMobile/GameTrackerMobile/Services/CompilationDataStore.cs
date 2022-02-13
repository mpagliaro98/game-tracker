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
            rm.AddGameCompilation(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(GameCompilation item)
        {
            var oldItem = rm.FindGameCompilation(new ObjectReference(item));
            rm.UpdateGameCompilation(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(GameCompilation item)
        {
            rm.DeleteGameCompilation(item);

            return await Task.FromResult(true);
        }

        public override async Task<GameCompilation> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindGameCompilation(key));
        }

        public override async Task<IEnumerable<GameCompilation>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.GameCompilations);
        }
    }
}
