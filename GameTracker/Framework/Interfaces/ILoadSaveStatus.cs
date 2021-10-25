using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface ILoadSaveStatus<TStatus>
        where TStatus : Status
    {
        IEnumerable<TStatus> LoadStatuses();
        void SaveStatuses(IEnumerable<TStatus> statuses);
    }
}
