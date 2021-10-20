using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineRatedStatus<TListedObj, TRange, TSettings, TStatus>
        : LoadSaveEngineRated<TListedObj, TRange, TSettings>, ILoadSaveStatus<TStatus>
        where TListedObj : RatableObjectStatus, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : Status, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_STATUSES = new LoadSaveIdentifier("Statuses");

        public virtual IEnumerable<TStatus> LoadStatuses()
        {
            return LoadISavableList<TStatus>(ID_STATUSES);
        }

        public virtual void SaveStatuses(IEnumerable<TStatus> statuses)
        {
            SaveISavableList(statuses, ID_STATUSES);
        }
    }
}
