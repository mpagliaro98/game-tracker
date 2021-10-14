using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineCompletable : LoadSaveEngine
    {
        public virtual IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule)
        {
            return LoadListParent<CompletionStatus>(parentModule);
        }

        public virtual void SaveCompletionStatuses(IEnumerable<CompletionStatus> completionStatuses)
        {
            SaveListParent(completionStatuses);
        }

        public override IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableObjectCompletable>(parentModule);
        }

        public override void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects)
        {
            SaveListParent(ratableObjects.Cast<RatableObjectCompletable>());
        }
    }
}
