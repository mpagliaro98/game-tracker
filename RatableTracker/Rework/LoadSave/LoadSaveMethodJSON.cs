using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveMethodJSON : ILoadSaveMethod
    {
        protected readonly ILoadSaveLocation loadSaveLocation;

        public LoadSaveMethodJSON(ILoadSaveLocation loadSaveLocation)
        {
            this.loadSaveLocation = loadSaveLocation;
        }

        public IList<RankedObject> LoadModelObjects()
        {
            throw new NotImplementedException();
        }

        public void SaveAllModelObjects(IList<RankedObject> rankedObjects)
        {
            throw new NotImplementedException();
        }

        public void SaveOneModelObject(RankedObject rankedObject)
        {
            throw new NotImplementedException();
        }
    }
}
