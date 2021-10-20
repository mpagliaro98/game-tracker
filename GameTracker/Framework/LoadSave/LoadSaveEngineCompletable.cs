using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineCompletable<TRatableObj, TRatingCat, TCompStatus>
        : LoadSaveEngine<TRatableObj, TRatingCat>
        where TRatableObj : RatableObjectCompletable, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
        where TCompStatus : CompletionStatus, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_COMPLETIONSTATUSES = new LoadSaveIdentifier("CompletionStatuses");

        public virtual IEnumerable<TCompStatus> LoadCompletionStatuses(RatingModuleCompletable<TRatableObj, TRatingCat, TCompStatus> parentModule)
        {
            return LoadListParent<TCompStatus>(parentModule, ID_COMPLETIONSTATUSES);
        }

        public virtual void SaveCompletionStatuses(IEnumerable<TCompStatus> completionStatuses)
        {
            SaveListParent(completionStatuses, ID_COMPLETIONSTATUSES);
        }
    }
}
