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
        : LoadSaveEngineRated<TListedObj, TRange, TSettings>, ILoadSaveStatus
        where TListedObj : RatableObjectStatus, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : Status, ISavable, new()
    {
        public LoadSaveIdentifier ID_STATUSES => new LoadSaveIdentifier("Statuses");
    }
}
