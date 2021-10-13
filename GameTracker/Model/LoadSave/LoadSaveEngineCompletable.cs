using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public abstract class LoadSaveEngineCompletable : LoadSaveEngine
    {
        public abstract IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule);
        public abstract void SaveCompletionStatuses(IEnumerable<CompletionStatus> completionStatuses);
    }
}
