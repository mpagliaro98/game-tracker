using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveHandler
    {
        protected readonly ILoadSaveLocation location;
        protected readonly ILoadSaveMethod method;

        public LoadSaveHandler(ILoadSaveLocation location, ILoadSaveMethod method)
        {
            this.location = location;
            this.method = method;
        }

        // want to have functions like this instead of generic "save all/load all" from before
        public void SaveOneModelObject(RankedObject rankedObject)
        {

        }
    }
}
