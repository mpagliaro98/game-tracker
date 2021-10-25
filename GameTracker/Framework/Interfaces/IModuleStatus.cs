using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IModuleStatus<TStatus> where TStatus : Status
    {
        int LimitStatuses { get; }
        IEnumerable<TStatus> Statuses { get; }
        void LoadStatuses();
        void SaveStatuses();
        TStatus FindStatus(ObjectReference objectKey);
        void AddStatus(TStatus obj);
        void UpdateStatus(TStatus obj, TStatus orig);
        void DeleteStatus(TStatus obj);
        IEnumerable<TStatus> SortStatuses<TField>(Func<TStatus, TField> keySelector, SortMode mode = SortMode.ASCENDING);
        void ValidateStatus(TStatus obj);
    }
}
