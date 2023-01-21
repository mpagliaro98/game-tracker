using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveMethod : IDisposable
    {
        void SaveOneModelObject(RankedObject rankedObject);
        void SaveAllModelObjects(IList<RankedObject> rankedObjects);
        void DeleteOneModelObject(RankedObject rankedObject);
        IList<RankedObject> LoadModelObjects(Settings settings, TrackerModule module);
        void SaveSettings(Settings settings);
        Settings LoadSettings();
        void SetCancel(bool cancel);
    }
}
