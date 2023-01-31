using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TrackerObjectExtensionAttribute : Attribute
    {
        public TrackerObjectExtensionAttribute() { }
    }
}
