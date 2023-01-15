using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveLocationLocal : ILoadSaveLocation
    {
        private readonly ILoadSaveMethod loadSaveMethod;

        public LoadSaveLocationLocal(ILoadSaveMethod loadSaveMethod)
        {
            this.loadSaveMethod = loadSaveMethod;
        }
    }
}
