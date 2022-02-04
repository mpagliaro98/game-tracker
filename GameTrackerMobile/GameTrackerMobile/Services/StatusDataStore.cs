﻿using GameTracker.Model;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMobile.Services
{
    public class StatusDataStore : BaseDataStore<CompletionStatus>
    {
        public StatusDataStore() : base() { }

        public override async Task<bool> AddItemAsync(CompletionStatus item)
        {
            rm.AddStatus(item);

            return await Task.FromResult(true);
        }

        public override async Task<bool> UpdateItemAsync(CompletionStatus item)
        {
            var oldItem = rm.FindStatus(new ObjectReference(item));
            rm.UpdateStatus(item, oldItem);

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteItemAsync(CompletionStatus item)
        {
            rm.DeleteStatus(item);

            return await Task.FromResult(true);
        }

        public override async Task<CompletionStatus> GetItemAsync(ObjectReference key)
        {
            return await Task.FromResult(rm.FindStatus(key));
        }

        public override async Task<IEnumerable<CompletionStatus>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(rm.Statuses);
        }
    }
}
