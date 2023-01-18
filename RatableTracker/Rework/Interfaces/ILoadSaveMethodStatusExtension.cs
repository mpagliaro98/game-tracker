using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveMethodStatusExtension : ILoadSaveMethod
    {
        void SaveOneStatus(Status status);
        void SaveAllStatuses(IList<Status> statuses);
        void DeleteOneStatus(Status status);
        IList<Status> LoadStatuses(StatusExtensionModule module);
    }
}
