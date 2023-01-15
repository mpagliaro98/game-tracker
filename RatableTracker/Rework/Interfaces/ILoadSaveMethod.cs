using RatableTracker.Rework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveMethod
    {
        void SaveOneModelObject(RankedObject rankedObject);
    }
}
