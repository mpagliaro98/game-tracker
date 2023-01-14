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
        Task LoadStatusesAsync();
        void SaveStatuses();
        Task SaveStatusesAsync();
        TStatus FindStatus(ObjectReference objectKey);
        void AddStatus(TStatus obj);
        void UpdateStatus(TStatus obj, TStatus orig);
        void DeleteStatus(TStatus obj);
        void ValidateStatus(TStatus obj);
    }
}
