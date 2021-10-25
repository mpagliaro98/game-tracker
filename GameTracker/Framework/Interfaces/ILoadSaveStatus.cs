using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework.Interfaces
{
    public interface ILoadSaveStatus
    {
        LoadSaveIdentifier ID_STATUSES { get; }
    }
}
