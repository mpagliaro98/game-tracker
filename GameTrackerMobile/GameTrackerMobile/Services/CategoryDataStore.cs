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
            RefreshModule();
            rm.AddRatingCategory(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(RatingCategoryWeighted item)
        {
            RefreshModule();
            var oldItem = rm.FindRatingCategory(new ObjectReference(item));
            rm.UpdateRatingCategory(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(RatingCategoryWeighted item)
        {
            RefreshModule();
            rm.DeleteRatingCategory(item);

            return await Task.FromResult(true);
        }

        public override async Task<RatingCategoryWeighted> GetItemAsync(ObjectReference key)
        {
            RefreshModule();
            return await Task.FromResult(rm.FindRatingCategory(key));
        }

        public override async Task<IEnumerable<RatingCategoryWeighted>> GetItemsAsync(bool forceRefresh = false)
        {
            RefreshModule();
            return await Task.FromResult(rm.RatingCategories);
        }
    }
}
