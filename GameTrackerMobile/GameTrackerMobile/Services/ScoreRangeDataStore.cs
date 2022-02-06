using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMobile.Services
{
    public class ScoreRangeDataStore : BaseDataStore<ScoreRange>
    {
        public ScoreRangeDataStore() : base() { }

        public override async Task<bool> AddItemAsync(ScoreRange item)
        {
            rm.AddRange(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(ScoreRange item)
        {
            var oldItem = rm.FindRange(new ObjectReference(item));
            rm.UpdateRange(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(ScoreRange item)
        {
            rm.DeleteRange(item);

            return await Task.FromResult(true);
        }

        public override async Task<ScoreRange> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindRange(key));
        }

        public override async Task<IEnumerable<ScoreRange>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.Ranges);
        }
    }
}
