using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SavableAttribute : Attribute
    {
        public string Key { get; init; }
        public bool HandleLoadManually { get; set; } = false;
        public bool HandleRestoreManually { get; set; } = false;
        public bool SaveOnly { get; set; } = false;

        public SavableAttribute([CallerMemberName] string key = "")
        {
            Key = key;
        }
    }
}
