using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ILoadSaveMethodStatusExtension : ILoadSaveMethod
    {
        void SaveOneStatus(Status status);
        void SaveAllStatuses(IList<Status> statuses);
        void DeleteOneStatus(Status status);
        IList<Status> LoadStatuses(IModuleStatus module, Settings settings);
    }
}
