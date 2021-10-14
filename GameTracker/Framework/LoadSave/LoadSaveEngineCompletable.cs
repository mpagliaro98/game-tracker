using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineCompletable : LoadSaveEngine
    {
        protected static LoadSaveIdentifier ID_COMPLETIONSTATUSES = new LoadSaveIdentifier("CompletionStatuses");

        public virtual IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule)
        {
            return LoadListParent<CompletionStatus>(parentModule, ID_COMPLETIONSTATUSES);
        }

        public virtual void SaveCompletionStatuses(IEnumerable<CompletionStatus> completionStatuses)
        {
            SaveListParent(completionStatuses, ID_COMPLETIONSTATUSES);
        }

        public override IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableObjectCompletable>(parentModule, ID_RATABLEOBJECTS);
        }
    }
}
