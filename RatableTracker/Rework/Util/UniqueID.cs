using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public struct UniqueID
    {
        private Guid uniqueKey;

        public static UniqueID NewID()
        {
            return new UniqueID
            {
                uniqueKey = Guid.NewGuid()
            };
        }

        public static UniqueID BlankID()
        {
            return new UniqueID
            {
                uniqueKey = Guid.Empty
            };
        }

        public static UniqueID Parse(ulong value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new UniqueID
            {
                uniqueKey = new Guid(bytes)
            };
        }

        public static UniqueID Parse(string value)
        {
            // TODO if string is in wrong format, just use whatever bytes are there
            return new UniqueID
            {
                uniqueKey = Guid.Parse(value)
            };
        }

        public static UniqueID Copy(UniqueID uniqueID)
        {
            return new UniqueID
            {
                uniqueKey = uniqueID.uniqueKey
            };
        }

        public static UniqueID Copy(IKeyable keyable)
        {
            return new UniqueID
            {
                uniqueKey = keyable.UniqueID.uniqueKey
            };
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
