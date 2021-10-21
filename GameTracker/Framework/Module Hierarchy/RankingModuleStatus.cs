using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RankingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        : RankingModule<TListedObj, TRange, TSettings>, IModuleStatus<TStatus>
        where TListedObj : ListedObjectStatus
        where TRange : ScoreRange
        where TSettings : Settings
        where TStatus : Status
    {
        protected IEnumerable<TStatus> statuses;
        public IEnumerable<TStatus> Statuses => statuses;

        public virtual int LimitStatuses => 20;

        public override void Init()
        {
            base.Init();
            LoadStatuses();
        }

        public abstract void LoadStatuses();
        public abstract void SaveStatuses();

        public TStatus FindStatus(ObjectReference objectKey)
        {
            return FindObject(statuses, objectKey);
        }

        public void AddStatus(TStatus obj)
        {
            AddToList(ref statuses, SaveStatuses, obj, LimitStatuses);
        }

        public void UpdateStatus(TStatus obj, TStatus orig)
        {
            UpdateInList(ref statuses, SaveStatuses, obj, orig);
        }

        public void DeleteStatus(TStatus obj)
        {
            DeleteFromList(ref statuses, SaveStatuses, obj);
            listedObjs.Where(ro => ro.RefStatus.HasReference() && ro.RefStatus.IsReferencedObject(obj))
                .ForEach(ro => ro.RemoveStatus());
            if (GlobalSettings.Autosave) SaveListedObjects();
        }
    }
}
