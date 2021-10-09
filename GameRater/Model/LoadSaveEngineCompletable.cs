using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    abstract class LoadSaveEngineCompletable : LoadSaveEngine
    {
        public abstract IEnumerable<CompletionStatus> LoadCompletionStatuses(RatingModule parentModule);
        public abstract void SaveCompletionStatuses(IEnumerable<CompletionStatus> completionStatuses);
    }
}
