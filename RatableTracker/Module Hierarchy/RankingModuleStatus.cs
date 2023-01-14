using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RankingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        : RankingModule<TListedObj, TRange, TSettings>, IModuleStatus<TStatus>
        where TListedObj : ListedObjectStatus
        where TRange : ScoreRange
        where TSettings : Settings, new()
        where TStatus : Status
    {
        protected IEnumerable<TStatus> statuses = new List<TStatus>();
        public IEnumerable<TStatus> Statuses => statuses;

        public virtual int LimitStatuses => 20;

        public override void Init()
        {
            LoadStatuses();
            base.Init();
        }

        public override async Task InitAsync()
        {
            await LoadStatusesAsync();
            await base.InitAsync();
        }

        public abstract void LoadStatuses();
        public abstract Task LoadStatusesAsync();
        public abstract void SaveStatuses();
        public abstract Task SaveStatusesAsync();

        public TStatus FindStatus(ObjectReference objectKey)
        {
            return FindObject(statuses, objectKey);
        }

        public void AddStatus(TStatus obj)
        {
            ValidateStatus(obj);
            AddToList(ref statuses, SaveStatuses, obj, LimitStatuses);
        }

        public void UpdateStatus(TStatus obj, TStatus orig)
        {
            ValidateStatus(obj);
            UpdateInList(ref statuses, SaveStatuses, obj, orig);
        }

        public void DeleteStatus(TStatus obj)
        {
            DeleteFromList(ref statuses, SaveStatuses, obj);
            listedObjs.Where(ro => ro.RefStatus.HasReference() && ro.RefStatus.IsReferencedObject(obj))
                .ForEach(ro => ro.RemoveStatus());
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public virtual void ValidateStatus(TStatus obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > Status.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + Status.MaxLengthName.ToString() + " characters");
        }
    }
}
