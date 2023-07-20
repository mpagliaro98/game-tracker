using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ObjAddOns
{
    public class Status : TrackerObjectBase, IColorContainer
    {
        [Savable()] public Color Color { get; set; } = new Color();

        public virtual bool HideScoreOfModelObject { get { return false; } }
        public virtual bool ExcludeModelObjectFromStats { get { return false; } }

        protected new IModuleStatus Module => (IModuleStatus)base.Module;

        public Status(IModuleStatus module, Settings settings) : base(settings, (TrackerModule)module) { }

        public Status(Status copyFrom) : base(copyFrom)
        {
            Color = copyFrom.Color;
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.Module.StatusExtension.SaveStatus(this, module, (ILoadSaveMethodStatusExtension)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.Module.StatusExtension.DeleteStatus(this, module, (ILoadSaveMethodStatusExtension)conn);
        }
    }
}
