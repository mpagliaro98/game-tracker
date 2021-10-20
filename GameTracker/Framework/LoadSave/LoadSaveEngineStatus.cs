using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.ObjectHierarchy;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngineStatus<TListedObj, TRange, TSettings, TStatus>
        : LoadSaveEngine<TListedObj, TRange, TSettings>, ILoadSaveStatus<TStatus>
        where TListedObj : RankableObjectStatus, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : Settings, ISavable, new()
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
