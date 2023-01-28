using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public abstract class ModuleExtensionBase : ModulePatternBase
    {
        public ModuleBase BaseModule { get; internal set; }

        public ModuleExtensionBase(ILoadSaveHandler<ILoadSaveMethod> loadSave) : base(loadSave) { }

        public ModuleExtensionBase(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger) : base(loadSave, logger) { }
    }
}
