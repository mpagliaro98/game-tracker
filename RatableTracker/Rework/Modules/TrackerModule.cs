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
        protected readonly LoadSaveHandler loadSave;

        protected IList<RankedObject> ModelObjects => new List<RankedObject>();

        public TrackerModule(LoadSaveHandler loadSave)
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
            // TODO validate, add, save
            // or put save function on RankedObject that calls the module to put it in the list?
        }

        public static T FindObjectInList<T>(IList<T> list, UniqueID uniqueID) where T : IKeyable
        {
            foreach (T item in list)
            {
                if (item.UniqueID.Equals(uniqueID)) return item;
            }
            return default; // null
        }
    }
}
