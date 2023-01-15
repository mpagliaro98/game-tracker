using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModule
    {
        public virtual int LimitListedObjects => 100000;

        protected IList<RankedObject> ModelObjects => new List<RankedObject>();

        protected readonly ILoadSaveMethod loadSave;

        public TrackerModule(ILoadSaveMethod loadSave)
        {
            this.loadSave = loadSave;
        }

        /// <summary>
        /// Get the list of listed objects, with any filtering and sorting applied.
        /// </summary>
        public IList<RankedObject> GetModelObjectList()
        {
            // TODO pass in filter and sort options
            return ModelObjects;
        }

        public void AddModelObject(RankedObject modelObject)
        {
            // TODO validate, add, save (keep in mind limit)
            // or put save function on RankedObject that calls the module to put it in the list?
        }

        public void DeleteModelObject(RankedObject modelObject)
        {
            // TODO delete, save
        }
    }
}
