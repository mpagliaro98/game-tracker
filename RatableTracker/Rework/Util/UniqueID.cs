using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public class UniqueID
    {
        protected Guid uniqueKey = Guid.NewGuid();

        public UniqueID() { }

        public UniqueID(bool newRandomID)
        {
            if (!newRandomID) uniqueKey = Guid.Empty;
        }

        public UniqueID(int val)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(val).CopyTo(bytes, 0);
            uniqueKey = new Guid(bytes);
        }

        public UniqueID(Guid val)
        {
            uniqueKey = val;
        }

        public UniqueID(IKeyable keyable)
        {
            uniqueKey = keyable.UniqueID.uniqueKey;
        }

        public bool HasValue()
        {
            return !uniqueKey.Equals(Guid.Empty);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is UniqueID)) return false;
            UniqueID other = (UniqueID)obj;
            return HasValue() && uniqueKey.Equals(other.uniqueKey);
        }

        public override int GetHashCode()
        {
            return uniqueKey.GetHashCode();
        }

        public override string ToString()
        {
            return uniqueKey.ToString();
        }
    }
}
