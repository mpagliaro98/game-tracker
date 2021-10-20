using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RatingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        : RatingModule<TListedObj, TRange, TSettings>, IModuleStatus<TStatus>
        where TListedObj : RatableObjectStatus
        where TRange : ScoreRange
        where TSettings : SettingsScore
        where TStatus : Status
    {
        protected IEnumerable<TStatus> statuses;
        public IEnumerable<TStatus> Statuses
        {
            get { return statuses; }
        }

        public virtual int LimitStatuses => 20;

        public override void Init()
        {
            LoadStatuses();
            base.Init();
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
