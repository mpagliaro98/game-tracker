using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public abstract class ModuleBase : ModulePatternBase
    {
        public ModuleBase(ILoadSaveHandler<ILoadSaveMethod> loadSave) : base(loadSave) { }

        public ModuleBase(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger) : base(loadSave, logger) { }
    }
}
