using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveLocationAWSS3 : ILoadSaveLocation
    {
        private readonly ILoadSaveMethod loadSaveMethod;

        public LoadSaveLocationAWSS3(ILoadSaveMethod loadSaveMethod)
        {
            this.loadSaveMethod = loadSaveMethod;
        }
    }
}
