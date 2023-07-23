using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayAttribute : Attribute
    {
        public string Name { get; set; } = "";

        public EnumDisplayAttribute() { }
    }
}
