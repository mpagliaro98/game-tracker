using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMobile.Services
{
    public class CategoryDataStore : BaseDataStore<RatingCategoryWeighted>
    {
        public CategoryDataStore() : base() { }

        public override async Task<bool> AddItemAsync(RatingCategoryWeighted item)
        {
            rm.AddRatingCategory(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(RatingCategoryWeighted item)
        {
            var oldItem = rm.FindRatingCategory(new ObjectReference(item));
            rm.UpdateRatingCategory(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(RatingCategoryWeighted item)
        {
            rm.DeleteRatingCategory(item);

            return await Task.FromResult(true);
        }

        public override async Task<RatingCategoryWeighted> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindRatingCategory(key));
        }

        public override async Task<IEnumerable<RatingCategoryWeighted>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.RatingCategories);
        }
    }
}
