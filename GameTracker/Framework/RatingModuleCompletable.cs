using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework
{
    public abstract class RatingModuleCompletable : RatingModule
    {
        protected IEnumerable<CompletionStatus> completionStatuses;

        public IEnumerable<CompletionStatus> CompletionStatuses
        {
            get { return completionStatuses; }
        }

        public override void Init()
        {
            base.Init();
            LoadCompletionStatuses();
        }

        protected abstract void LoadCompletionStatuses();
        public abstract void SaveCompletionStatuses();

        public CompletionStatus FindCompletionStatus(ObjectReference objectKey)
        {
            return FindObject(completionStatuses, objectKey);
        }

        public void AddCompletionStatus(CompletionStatus obj)
        {
            AddToList(ref completionStatuses, SaveCompletionStatuses, obj);
        }

        public void UpdateCompletionStatus(CompletionStatus obj, CompletionStatus orig)
        {
            UpdateInList(ref completionStatuses, SaveCompletionStatuses, obj, orig);
        }

        public void DeleteCompletionStatus(CompletionStatus obj)
        {
            DeleteFromList(ref completionStatuses, SaveCompletionStatuses, obj);
            ratableObjects.Cast<RatableObjectCompletable>()
                .Where(ro => ro.CompletionStatus.Equals(obj))
                .ForEach(ro => ro.RemoveCompletionStatus());
            if (GlobalSettings.Autosave) SaveRatableObjects();
        }
    }
}
