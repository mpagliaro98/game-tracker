using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public abstract class ExtensionBase : IDisposable
    {
        protected ModuleExtensionBase Module { get; init; }
        protected Settings Settings { get; init; }
        public TrackerObjectBase BaseObject { get; internal set; }

        public ExtensionBase(ModuleExtensionBase module, Settings settings)
        {
            this.Module = module;
            this.Settings = settings;
        }

        public ExtensionBase(ExtensionBase copyFrom) : this(copyFrom.Module, copyFrom.Settings) { }

        protected internal virtual void ValidateFields() { }

        public void InitAdditionalResources()
        {
            AddEventHandlers();
        }

        protected virtual void AddEventHandlers() { }

        public void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers() { }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation sr) { }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr) { }
    }
}
