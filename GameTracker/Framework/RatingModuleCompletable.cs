using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework
{
    public abstract class RatingModuleCompletable<TRatableObj, TRatingCat, TCompStatus>
        : RatingModule<TRatableObj, TRatingCat>
        where TRatableObj : RatableObjectCompletable
        where TRatingCat : RatingCategory
        where TCompStatus : CompletionStatus
    {
        protected IEnumerable<TCompStatus> completionStatuses;

        public virtual int LimitCompletionStatuses => 20;

        public IEnumerable<TCompStatus> CompletionStatuses
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

        public TCompStatus FindCompletionStatus(ObjectReference objectKey)
        {
            return FindObject(completionStatuses, objectKey);
        }

        public void AddCompletionStatus(TCompStatus obj)
        {
            AddToList(ref completionStatuses, SaveCompletionStatuses, obj, LimitCompletionStatuses);
        }

        public void UpdateCompletionStatus(TCompStatus obj, TCompStatus orig)
        {
            UpdateInList(ref completionStatuses, SaveCompletionStatuses, obj, orig);
        }

        public void DeleteCompletionStatus(TCompStatus obj)
        {
            DeleteFromList(ref completionStatuses, SaveCompletionStatuses, obj);
            ratableObjects.Where(ro => ro.RefCompletionStatus.HasReference() && ro.RefCompletionStatus.IsReferencedObject(obj))
                .ForEach(ro => ro.RemoveCompletionStatus());
            if (GlobalSettings.Autosave) SaveRatableObjects();
        }
    }
}
